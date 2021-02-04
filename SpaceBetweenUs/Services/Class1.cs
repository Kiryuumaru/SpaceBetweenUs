//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using System.Security;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.VisualBasic;
//using System.Drawing.Drawing2D;

//public class Form1
//{
//    // This is the projected quadrilateral
//    private Point[] P = new Point[4];

//    // These are the homographic coefficients
//    private float A, B, D, E, G, H;

//    private void SolvePerspective()
//    {
//        float T;

//        // Compute the transform coefficients
//        T = (P[2].X - P[1].X) * (P[2].Y - P[3].Y) - (P[2].X - P[3].X) * (P[2].Y - P[1].Y);

//        G = ((P[2].X - P[0].X) * (P[2].Y - P[3].Y) - (P[2].X - P[3].X) * (P[2].Y - P[0].Y)) / (double)T;
//        H = ((P[2].X - P[1].X) * (P[2].Y - P[0].Y) - (P[2].X - P[0].X) * (P[2].Y - P[1].Y)) / (double)T;

//        A = G * (P[1].X - P[0].X);
//        D = G * (P[1].Y - P[0].Y);
//        B = H * (P[3].X - P[0].X);
//        E = H * (P[3].Y - P[0].Y);

//        G -= 1;
//        H -= 1;
//    }

//    private Point Perspective(float U, float V)
//    {
//        float T;

//        // Evaluate the homographic transform
//        T = G * U + H * V + 1;
//        return new Point((A * U + B * V) / (double)T + P[0].X, (D * U + E * V) / (double)T + P[0].Y);
//    }

//    private void Form1_Paint(System.Object sender, System.Windows.Forms.PaintEventArgs e)
//    {
//        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

//        try
//        {
//            // Draw the perspective interpolation
//            SolvePerspective();

//            // Draw the perspective grid
//            for (var U = 0.0625; U <= 1 - 0.0625; U += 0.0625)
//                e.Graphics.DrawLine(Pens.Green, Perspective(U, 0), Perspective(U, 1));
//            for (var V = 0.0625; V <= 1 - 0.0625; V += 0.0625)
//                e.Graphics.DrawLine(Pens.Green, Perspective(0, V), Perspective(1, V));
//        }
//        catch (Exception ex)
//        {
//        }

//        // Draw the outline
//        for (var i = 0; i <= 3; i++)
//            e.Graphics.DrawLine(Pens.Blue, P[i].X, P[i].Y, P[(i + 1) % 4].X, P[(i + 1) % 4].Y);
//    }

//    private void Form1_Load(System.Object sender, System.EventArgs e)
//    {
//        // Intialize the corners
//        P[0].X = Width / (double)4;
//        P[0].Y = Height / (double)4;
//        P[1].X = (3 * Width) / (double)4;
//        P[1].Y = Height / (double)4;
//        P[2].X = (3 * Width) / (double)4;
//        P[2].Y = (3 * Height) / (double)4;
//        P[3].X = Width / (double)4;
//        P[3].Y = (3 * Height) / (double)4;

//        DoubleBuffered = true;
//    }

//    private int Hit;

//    private void Form1_MouseMove(System.Object sender, System.Windows.Forms.MouseEventArgs e)
//    {
//        if (e.Button == Windows.Forms.MouseButtons.None)
//        {
//            // Hit a corner ?
//            for (Hit = 0; Hit <= 3; Hit++)
//            {
//                if ((P[Hit].X - e.X) * (P[Hit].X - e.X) + (P[Hit].Y - e.Y) * (P[Hit].Y - e.Y) < 100)
//                    break;
//            }

//            // Uodate the cursor
//            if (Hit < 4)
//                Cursor = Cursors.SizeAll;
//            else
//                Cursor = Cursors.Default;
//        }
//        else if (Hit < 4)
//        {
//            // Drag the corner and repaint
//            P[Hit].X = e.X;
//            P[Hit].Y = e.Y;
//            Refresh();
//        }
//    }
//}
