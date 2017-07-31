﻿using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace JiHuangBaiKeForUWP.UserControls.Charts
{
    public sealed partial class BarChart : UserControl
    {
        public BarChart()
        {
            this.InitializeComponent();
        }

        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(BarChart), new PropertyMetadata(1));
        #region 依赖属性：值

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(BarChart), new PropertyMetadata(false, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var barChart = (BarChart)d;
                if ((double)e.NewValue != 0)
                {
                    barChart.ValueTextBlock.Text = e.NewValue.ToString();
                }
                else
                {
                    barChart.Visibility = Visibility.Collapsed;
                }
                if ((double)e.NewValue < 0)
                {
                    barChart.ValueTextBlock.Foreground = new SolidColorBrush(Colors.White);
                    barChart.ValueRectangle.Width = -(double)e.NewValue / barChart.MaxValue * 300;
                }
                else
                {
                    barChart.ValueRectangle.Width = (double)e.NewValue / barChart.MaxValue * 300;
                }
            }
        }
        #endregion

        #region 依赖属性：标签

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(BarChart), new PropertyMetadata(false, OnLabelChanged));

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var barChart = (BarChart)d;
                barChart.LabelTextBlock.Text = (string)e.NewValue;
            }
        }

        #endregion

        #region 依赖属性：BarChart颜色

        public SolidColorBrush BarColor
        {
            get => (SolidColorBrush)GetValue(BarColorProperty);
            set => SetValue(BarColorProperty, value);
        }

        public static readonly DependencyProperty BarColorProperty =
            DependencyProperty.Register("BarColor", typeof(SolidColorBrush), typeof(BarChart), new PropertyMetadata(false, OnBarColorChanged));

        private static void OnBarColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var barChart = (BarChart)d;
                barChart.ValueRectangle.Fill = (SolidColorBrush)e.NewValue;
            }
        }

        #endregion

    }
}