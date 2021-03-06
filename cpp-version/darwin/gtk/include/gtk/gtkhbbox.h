/* GTK - The GIMP Toolkit
 * Copyright (C) 1995-1997 Peter Mattis, Spencer Kimball and Josh MacDonald
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

/*
 * Modified by the GTK+ Team and others 1997-2000.  See the AUTHORS
 * file for a list of people on the GTK+ Team.  See the ChangeLog
 * files for a list of changes.  These files are distributed with
 * GTK+ at ftp://ftp.gtk.org/pub/gtk/. 
 */

#ifndef __GTK_HBUTTON_BOX_H__
#define __GTK_HBUTTON_BOX_H__


#include <gtk/gtkbbox.h>


#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */


#define GTK_TYPE_HBUTTON_BOX                  (gtk_hbutton_box_get_type ())
#define GTK_HBUTTON_BOX(obj)                  (G_TYPE_CHECK_INSTANCE_CAST ((obj), GTK_TYPE_HBUTTON_BOX, GtkHButtonBox))
#define GTK_HBUTTON_BOX_CLASS(klass)          (G_TYPE_CHECK_CLASS_CAST ((klass), GTK_TYPE_HBUTTON_BOX, GtkHButtonBoxClass))
#define GTK_IS_HBUTTON_BOX(obj)               (G_TYPE_CHECK_INSTANCE_TYPE ((obj), GTK_TYPE_HBUTTON_BOX))
#define GTK_IS_HBUTTON_BOX_CLASS(klass)       (G_TYPE_CHECK_CLASS_TYPE ((klass), GTK_TYPE_HBUTTON_BOX))
#define GTK_HBUTTON_BOX_GET_CLASS(obj)        (G_TYPE_INSTANCE_GET_CLASS ((obj), GTK_TYPE_HBUTTON_BOX, GtkHButtonBoxClass))


typedef struct _GtkHButtonBox       GtkHButtonBox;
typedef struct _GtkHButtonBoxClass  GtkHButtonBoxClass;

struct _GtkHButtonBox
{
  GtkButtonBox button_box;
};

struct _GtkHButtonBoxClass
{
  GtkButtonBoxClass parent_class;
};


GType      gtk_hbutton_box_get_type (void) G_GNUC_CONST;
GtkWidget* gtk_hbutton_box_new      (void);

/* buttons can be added by gtk_container_add() */

#ifndef GTK_DISABLE_DEPRECATED
gint gtk_hbutton_box_get_spacing_default (void);
GtkButtonBoxStyle gtk_hbutton_box_get_layout_default (void);

void gtk_hbutton_box_set_spacing_default (gint spacing);
void gtk_hbutton_box_set_layout_default (GtkButtonBoxStyle layout);
#endif

#ifdef __cplusplus
}
#endif /* __cplusplus */


#endif /* __GTK_HBUTTON_BOX_H__ */
