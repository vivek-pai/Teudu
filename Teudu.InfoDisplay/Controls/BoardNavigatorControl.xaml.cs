﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Teudu.InfoDisplay
{
    /// <summary>
    /// Interaction logic for BoardNavigatorControl.xaml
    /// </summary>
    public partial class BoardNavigatorControl : UserControl
    {
        public BoardNavigatorControl()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(BoardNavigatorControl_Loaded);
        }

        void BoardNavigatorControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = this.ActualWidth;
            CurrentBoard.Width = App.Current.MainWindow.ActualWidth;
            CurrentBoard.MaxWidth = App.Current.MainWindow.ActualWidth;
        }

        private IBoardService boardMaster;
        public IBoardService BoardMaster
        {
            set 
            { 
                //stop monitoring interactions
                boardMaster = value;
                Current = boardMaster.Current;
                Next = boardMaster.Next;
                //JumpToCenter();
                //begin monitoring
            }
        }

        private Board prev, current, next;
        public Board Previous
        {
            set
            {
                prev = value;
                this.Dispatcher.BeginInvoke(new Action(this.LoadPreviousBoard), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void LoadPreviousBoard()
        {
            //do slick animation
            PreviousBoard.BoardModel = prev;
        }

        public Board Current
        {
            set
            {
                current = value;
                CurrentBoard.BoardModel = current;
            }
        }

        public Board Next
        {
            set
            {
                next = value;
                this.Dispatcher.BeginInvoke(new Action(this.LoadNextBoard), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void LoadNextBoard()
        {
            //do slick animation
            NextBoard.BoardModel = next;
        }

        private void Advance()
        {
            if (boardMaster == null || !boardMaster.AdvanceCurrent())
                return;

            BindingOperations.ClearAllBindings(PanPosition);
            PanPosition.Changed -= new EventHandler(TranslateTransform_Changed);
            ((System.Windows.Media.Animation.Storyboard)this.Resources["AdvanceAnimation"]).Begin();     
            //DoubleAnimation anim1 = new DoubleAnimation(-1920,-
        }

        private void SetTranslateBindings()
        {
            OffsetNavigatorConverter con = (OffsetNavigatorConverter)App.Current.Resources["offsetConverter"];
            Binding bindingX = new Binding
            {
                Path = new PropertyPath("DominantArmHandOffsetX"),
                Converter = con,
                ConverterParameter = -1920
            };
            Binding bindingY = new Binding
            {
                Path = new PropertyPath("DominantArmHandOffsetY")
            };
            BindingOperations.SetBinding(PanPosition, TranslateTransform.XProperty, bindingX);
            BindingOperations.SetBinding(PanPosition, TranslateTransform.YProperty, bindingY);
        }

        private void Regress()
        {
            if (boardMaster == null || !boardMaster.RegressCurrent())
                return;
            //animation, completed => refresh prev, next boards
            Current = prev;
            //jump to current board
            JumpToCenter();
            Next = current;
            Previous = boardMaster.Prev;
        }

        private void JumpToCenter()
        {
            //TranslateTransform shift = new TranslateTransform(-this.ActualWidth, 0);
            //BoardContainer.RenderTransform = shift;
            Canvas.SetLeft(BoardContainer, -this.ActualWidth);
        }

        public Point BoardContainerOffset
        {
            get
            {
                return BoardContainer.TransformToAncestor(this).Transform(new Point(0, 0));
            }
        }

        private bool GoPrevious()
        {
            //Point containerCenter = this.TransformToAncestor(App.Current.MainWindow).Transform(new Point(BoardContainer.ActualWidth/2,0));
            return BoardMidLocation().X > BoardContainer.ActualWidth/2;
        }

        private bool GoNext()
        {
            //System.Diagnostics.Trace.WriteLine("mid loc: " + BoardMidLocation().X);
            return (BoardMidLocation().X) < 0; ///TODO: change to be based on event width
        }

        private Point BoardMidCoords()
        {
            double midY = this.BoardContainer.ActualHeight / 2;
            double midX = this.BoardContainer.ActualWidth / 2;

            return new Point(midX, midY);
        }

        private Point BoardMidLocation()
        {
            return BoardContainer.TransformToAncestor(this).Transform(BoardMidCoords());
        }

        private void TranslateTransform_Changed(object sender, EventArgs e)
        {
            //System.Diagnostics.Trace.WriteLine("mid of board :" + BoardMidLocation().X);
            if (GoNext())
                Advance();
            else if (GoPrevious())
                Regress();
        }

        private void AdvanceAnimation_Completed(object sender, EventArgs e)
        {
            Previous = current;
            Current = next;
            ////jump to current board
            JumpToCenter();
            Next = boardMaster.Next;
            SetTranslateBindings();
            PanPosition.Changed += new EventHandler(TranslateTransform_Changed);
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            double d = Canvas.GetLeft(BoardContainer);
            if (e.Key == Key.Left)
            {
                System.Diagnostics.Trace.WriteLine("left!");
                Canvas.SetLeft(BoardContainer, d - 5);
                //window1.Left -= d;
            }
            if (e.Key == Key.Right)
            {
                Canvas.SetLeft(BoardContainer, d + 5);
                //window1.Left += d;
            }
            if (e.Key == Key.Up)
            {
                //window1.Top -= d;
            }
            if (e.Key == Key.Down)
            {
                //window1.Top += d;
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            
        }
    }
}