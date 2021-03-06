﻿//                                            *
//   file: DatabaseFin.h
//
// author: Adam Russell
//
//   mods: J H Stewman (9/27/2005)
//         -- code to determine whether particular databasefin
//            is being used for an UNKNOWN or is being created as it
//            is read from the database
//            ASSUMPTION: if image filename contains any slashes it is
//            presumed to be an UNKNOWN
//
//                                            *

// This file is part of DARWIN.
// Copyright (C) 1994 - 2020
//
// DARWIN is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// DARWIN is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with DARWIN.  If not, see<https://www.gnu.org/licenses/>.

//
// 0.3.1-DB: Addendum : New DatabaseFin Data structure
// [Data Position] (4 Bytes)
// [Image Filename] (char[255]) **Delimited by '\n'
// [Number of Contour Points] (4 bytes)
// [Contour Points ...] (Number * (int) bytes)
// [Thumbnail Pixmap] (25*25)
// [Short Description] (char[255]) **Delimited by '\n'
//
// Darwin_0.3.8 - DB version 2: Addendum : New DatabaseFin Data structure
// [Data Position] (4 Bytes) -- or "DFIN" as hex number in saved traced fin files
// [Image Filename] (char[255]) **Delimited by '\n'
// [Number of FloatContour Points] (4 bytes)
// [FloatContour Points ...] (Number*2*sizeof(float) bytes)
// [Feature Point Positions] (5*sizeof(int) bytes)
// [Thumbnail Pixmap] (25*25 bytes)
// [Short Description] (char[255]) **Delimited by '\n'
//
// Darwin_1.4 - DB version 4: Addendum : New DatabaseFin Data structure
// this adds fields for tracking changes to image while tracing fin
// [Data Position] (4 Bytes) -- or "DFIN" as hex number in saved traced fin files
// [Image Filename] (char[255]) **Delimited by '\n'
// [Number of FloatContour Points] (4 bytes)
// [FloatContour Points ...] (Number*2*sizeof(float) bytes)
// [Feature Point Positions] (5*sizeof(int) bytes)
// [Thumbnail Pixmap] (25*25 bytes)
// [Is Left Side] '1' or '0'
// [Is Flipped Image] '1' or '0'
// [Clipping bounds xmin,ymin,xmax,ymax] (4 * sizeof(double))
// [Normalizing Scale] (sizeof(double)
// [Alternate (blind) ID] (5 chars) **Delimited by '\n'
// [Short Description] (char[255]) **Delimited by '\n'
//

using Darwin.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace Darwin.Database
{
    public class DatabaseFin : BaseEntity, INotifyPropertyChanged
    {
        public Bitmap OriginalFinImage;
        public Bitmap FinImage;      // modified fin image from TraceWin, ...

        //public long DatabaseID { get; set; }

        private Outline _finOutline;
        private string _IDCode;
        private string _name;
        private string _dateOfSighting;
        private string _rollAndFrame;
        private string _locationCode;
        private string _damageCategory;
        private string _shortDescription;
        private string _imageFilename; //  001DB
        private string _thumbnailFileName;
        public char[,] ThumbnailPixmap;
        public int ThumbnailRows;

        //  1.4 - new members for tracking image modifications during tracing
        public bool mLeft, mFlipped;              // left side or flipped internally to swim left
        public double XMin, YMin, XMax, YMax; // internal cropping bounds
        public double Scale;                     // image to Outline scale change

        public List<ImageMod> ImageMods;    //  1.8 - for list of image modifications
        public string OriginalImageFilename; //  1.8 - filename of original unmodified image

        private string _finFilename;

        public string FinFilename   //  1.6 - for name of fin file if fin saved outside DB
        {
            get => _finFilename;
            set
            {
                _finFilename = value;
                RaisePropertyChanged("FinFilename");

                if (string.IsNullOrEmpty(OriginalImageFilename))
                    OriginalImageFilename = value;
            }
        }

        public string FinFilenameOnly
        {
            get
            {
                if (_finFilename == null)
                    return null;

                return Path.GetFileName(_finFilename);
            }
        }

        public bool IsAlternate; //  1.95 - allow designation of primary and alternate fins/images

        private bool _fieldsChanged;
        public bool FieldsChanged
        {
            get => _fieldsChanged;
            set
            {
                _fieldsChanged = value;
                RaisePropertyChanged("FieldsChanged");
            }
        }

        public string IDCode
        {
            get => _IDCode;
            set
            {
                _IDCode = value;
                RaisePropertyChanged("IDCode");
                FieldsChanged = true;
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
                FieldsChanged = true;
            }
        }
        public string DateOfSighting
        {
            get => _dateOfSighting;
            set
            {
                _dateOfSighting = value;
                RaisePropertyChanged("DateOfSighting");
                FieldsChanged = true;
            }
        }

        public string RollAndFrame
        {
            get => _rollAndFrame;
            set
            {
                _rollAndFrame = value;
                RaisePropertyChanged("RollAndFrame");
                FieldsChanged = true;
            }
        }

        public string LocationCode
        {
            get => _locationCode;
            set
            {
                _locationCode = value;
                RaisePropertyChanged("LocationCode");
                FieldsChanged = true;
            }
        }

        public string DamageCategory
        {
            get => _damageCategory;
            set
            {
                _damageCategory = value;
                RaisePropertyChanged("DamageCategory");
                FieldsChanged = true;
            }
        }

        public string ShortDescription
        {
            get => _shortDescription;
            set
            {
                _shortDescription = value;
                RaisePropertyChanged("ShortDescription");
                FieldsChanged = true;
            }
        }

        public string ImageFilename
        {
            get => _imageFilename;
            set
            {
                _imageFilename = value;
                RaisePropertyChanged("ImageFilename");
            }
        }

        public string ThumbnailFilename
        {
            get => _thumbnailFileName;
            set
            {
                _thumbnailFileName = value;
                RaisePropertyChanged("ThumbnailFilename");
                RaisePropertyChanged("ThumbnailFilenameUri");
            }
        }

        public string ThumbnailFilenameUri
        {
            get
            {
                if (string.IsNullOrEmpty(ThumbnailFilename))
                    return null;

                return Path.Combine(Options.CurrentUserOptions.CurrentSurveyAreaPath, ThumbnailFilename);
            }
        }

        public Outline FinOutline //  008OL
        {
            get => _finOutline;
            set
            {
                _finOutline = value;
                RaisePropertyChanged("FinOutline");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DatabaseFin()
        {

        }

        //                                                **
        //
        // called from numerous places in ...
        //   TraceWindow.cxx, ModifyDatabaseWindow.cxx, and 
        //   NoMatchWindow.cxx
        //
        public DatabaseFin(
            string filename, //  001DB
            Outline outline, //  008OL
            string idcode,
            string name,
            string dateOfSighting,
            string rollAndFrame,
            string locationCode,
            string damageCategory,
            string shortDescription
        )
        {
            ImageFilename = filename; //  001DB
            FinOutline = new Outline(outline); //  006DF,008OL
            IDCode = idcode;
            Name = name;
            DateOfSighting = dateOfSighting;
            RollAndFrame = rollAndFrame;
            LocationCode = locationCode;
            DamageCategory = damageCategory;
            ShortDescription = shortDescription;
            ThumbnailPixmap = null;
            ThumbnailRows = 0;
            mLeft = true; //  1.4
            mFlipped = false; //  1.4
            XMin = 0.0; //  1.4
            XMax = 0.0; //  1.4
            YMin = 0.0; //  1.4
            YMax = 0.0; //  1.4
            Scale = 1.0; //  1.4
            FinImage = null; //  1.5
            FinFilename = string.Empty; //  1.6
            IsAlternate = false; //  1.95

            //  1.5 - need some way to CATCH error thrown when image file
            //         does not exist or is unsupported type  --
            //         program now crashes when database image is misplaced or misnamed

            OriginalFinImage = new Bitmap(ImageFilename); //  001DB

            FieldsChanged = false;
            // TODO
            //FIN_IMAGE_TYPE* thumb = resizeWithBorderNN(
            //		mFinImage,
            //		DATABASEFIN_THUMB_HEIGHT,
            //		DATABASEFIN_THUMB_WIDTH);

            // TODO
            //convToPixmapString(thumb, mThumbnailPixmap, mThumbnailRows);
        }

        //  1.99 - new constructor used by SQLlite database code
        //                                                **
        //
        // Added. Called in Database::getFin().
        //
        public DatabaseFin(
            long id,
            string filename, //  001DB
            Outline outline, //  008OL
			string idcode,
			string name,
			string dateOfSighting,
			string rollAndFrame,
			string locationCode,
			string damageCategory,
			string shortDescription,
			long datapos
		)
        {
            ID = id;
            ImageFilename = filename; //  001DB
			FinOutline = new Outline(outline); //  006DF,008OL
            IDCode = idcode;
            Name = name;
            DateOfSighting = dateOfSighting;
            RollAndFrame = rollAndFrame;
            LocationCode = locationCode;
            DamageCategory = damageCategory;
            ShortDescription = shortDescription;
            ID = datapos;
            mLeft = true; //  1.4
            mFlipped = false; //  1.4
            XMin = 0.0; //  1.4
            XMax = 0.0; //  1.4
            YMin= 0.0; //  1.4
            YMax = 0.0; //  1.4
            Scale = 1.0; //  1.4
            FinImage = null; //  1.5
            FinFilename = string.Empty; //  1.6
            OriginalFinImage = null;
            IsAlternate = false; //  1.99

            FieldsChanged = false;
            // let's see what happens... -- rjn
            /*
            mFinImage=new FIN_IMAGE_TYPE(mImageFilename); //  001DB
            FIN_IMAGE_TYPE *thumb = resizeWithBorderNN(
                    mFinImage,
                    DATABASEFIN_THUMB_HEIGHT,
                    DATABASEFIN_THUMB_WIDTH);
            convToPixmapString(thumb, m
            
            Pixmap, mThumbnailRows);
            delete thumb;
            */
        }

        //                                                **
        //
        // called ONLY from Match.cxx and MatchResultsWindow.cxx
        //
        public DatabaseFin(DatabaseFin fin)
        {
            ImageFilename = fin.ImageFilename;        //  001DB
			OriginalFinImage = null;                          //   major change JHS
            FinImage = null; //  1.5
            ID = fin.ID;                    //  001DB
            FinOutline = new Outline(fin.FinOutline); //  006DF,008OL
            IDCode = fin.IDCode;
            Name = fin.Name;
            DateOfSighting = fin.DateOfSighting;
            RollAndFrame = fin.RollAndFrame;
            LocationCode = fin.LocationCode;
            DamageCategory = fin.DamageCategory;
            ShortDescription = fin.ShortDescription;
            // TODO
            ThumbnailPixmap = null; //new char*[fin.mThumbnailRows];
            ThumbnailRows = fin.ThumbnailRows;
            mLeft = fin.mLeft; //  1.4
            mFlipped = fin.mFlipped; //  1.4
            XMin = fin.XMin; //  1.4
            XMax = fin.XMax; //  1.4
            YMin = fin.YMin; //  1.4
            YMax = fin.YMax; //  1.4
            Scale = fin.Scale; //  1.4
            FinFilename = fin.FinFilename; //  1.6
            OriginalImageFilename = fin.OriginalImageFilename; //  1.8

            ImageMods = fin.ImageMods; //  1.8

            IsAlternate = fin.IsAlternate; //  1.95

            //  1.5 - just set pointer to original copy from TraceWindow
            //  1.8 - we actually create a COPY of the modified image here
            if (null != fin.FinImage)
                FinImage = new Bitmap(fin.FinImage);

            //  1.8 - and we create a COPY of the original image here
            if (null != fin.OriginalFinImage)
                OriginalFinImage = new Bitmap(fin.OriginalFinImage);

            // TODO
            //for (int i = 0; i < fin.mThumbnailRows; i++)
            //{
            //    mThumbnailPixmap[i] = new char[strlen(fin->mThumbnailPixmap[i]) + 1];
            //    strcpy(mThumbnailPixmap[i], fin->mThumbnailPixmap[i]);
            //}

            FieldsChanged = false;
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SaveSightingData(string filename)
        {
            using (StreamWriter writer = File.AppendText(filename))
            {
                writer.WriteLine(
                    IDCode?.StripCRLFTab() + "\t" +
                    Name?.StripCRLFTab() + "\t" +
                    DateOfSighting?.StripCRLFTab() + "\t" +
                    RollAndFrame?.StripCRLFTab() + "\t" +
                    LocationCode?.StripCRLFTab() + "\t" +
                    DamageCategory?.StripCRLFTab() + "\t" +
                    ShortDescription?.StripCRLFTab() + "\t" +
                    OriginalImageFilename + "\t" +
                    ImageFilename);
            }
        }
    }
}
