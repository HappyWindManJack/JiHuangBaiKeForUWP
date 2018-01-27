﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using JiHuangBaiKeForUWP.Model;
using JiHuangBaiKeForUWP.UserControls;
using Windows.UI.Xaml.Media.Animation;

namespace JiHuangBaiKeForUWP.View.Dialog
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NaturalSmallPlantDialog : Page
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Global.GetOsVersion() >= 16299)
            {
                var dimGrayAcrylicBrush = new AcrylicBrush
                {
                    BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
                    FallbackColor = Colors.Transparent,
                    TintColor = Global.TinkColor,
                    TintOpacity = Global.TinkOpacity
                };
                RootScrollViewer.Background = dimGrayAcrylicBrush;
            }
            base.OnNavigatedTo(e);
            Global.FrameTitle.Text = "自然详情";
            if (e.Parameter != null)
            {
                LoadData((NatureSmallPlant)e.Parameter);
            }
            var imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Image");
            imageAnimation?.TryStart(NatureImage);
        }

        public NaturalSmallPlantDialog()
        {
            this.InitializeComponent();
        }

        private List<List<string>> _smallPlantResourceListStringList;
        private byte _smallPlantIndex;
        private int _smallPlantMaxIndex;
        private readonly List<string> _smallPlantList = new List<string>();

        private void LoadData(NatureSmallPlant c)
        {
            // 图片
            if (c.Pictures.Count == 0)
            {
                NatureImage.Source = new BitmapImage(new Uri(c.Picture));
            }
            else
            {
                SwitchLeftButton.Visibility = Visibility.Visible;
                SwitchRightButton.Visibility = Visibility.Visible;
                // 数量分割点
                var breakPosition = c.Pictures[0].IndexOf('|');
                // 多名称多图
                if (breakPosition == -1)
                {
                    _smallPlantMaxIndex = c.Pictures.Count - 1;
                    foreach (var pic in c.Pictures)
                    {
                        _smallPlantList.Add(StringProcess.GetGameResourcePath(pic));
                    }
                    NatureImage.Source = new BitmapImage(new Uri(_smallPlantList[0]));
                    SwitchLeftButton.IsEnabled = false;
                }
                // 单名称多图
                else
                {
                    var pictureText = c.Pictures[0].Substring(0, breakPosition);
                    var pictureNum = int.Parse(c.Pictures[0].Substring(breakPosition + 1));
                    _smallPlantMaxIndex = pictureNum - 1;
                    for (var i = 1; i <= pictureNum; i++)
                    {
                        _smallPlantList.Add(StringProcess.GetGameResourcePath(pictureText + i));
                    }
                    NatureImage.Source = new BitmapImage(new Uri(_smallPlantList[0]));
                    SwitchLeftButton.IsEnabled = false;
                }
            }
            //中英文名
            NatureName.Text = c.Name;
            NatureEnName.Text = c.EnName;
            // 可再生、可燃
            if (c.IsRegenerate)
            {
                RegenerateCheckBox.IsChecked = true;
            }
            if (c.IsCombustible)
            {
                CombustibleCheckBox.IsChecked = true;
            }
            // 资源
            if (c.Resources.Count == 0)
            {
                NaturalResourceTextBlock.Visibility = Visibility.Collapsed;
                NaturalResourceStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                _smallPlantResourceListStringList = new List<List<string>>();
                if (c.Resources.Count == 1)
                {
                    for (var i = 0; i <= _smallPlantMaxIndex; i++)
                        _smallPlantResourceListStringList.Add(c.Resources[0]);
                }
                else
                {
                    foreach (var strList in c.Resources)
                    {
                        _smallPlantResourceListStringList.Add(strList);
                    }
                }
                ShowResources(0);
            }
            // 烧毁后资源
            if (c.ResourcesBurned.Count == 0)
            {
                NaturalResourcesBurnedTextBlock.Visibility = Visibility.Collapsed;
                NaturalResourcesBurnedStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                var thickness = new Thickness(20, 0, 0, 0);
                foreach (var str in c.ResourcesBurned)
                {
                    //数量分割点
                    var breakPosition = str.IndexOf('|');
                    // 图片
                    var resourceSource = str.Substring(0, breakPosition);
                    // 数量文本
                    var resourceText = str.Substring(breakPosition + 1);
                    var picButton = new PicButton
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = thickness,
                        Source = StringProcess.GetGameResourcePath(resourceSource),
                        Text = resourceText
                    };
                    //picButton.Click += Creature_Jump_Click;
                    NaturalResourcesBurnedStackPanel.Children.Add(picButton);
                }
            }
            //特殊能力
            if (c.Ability.Count == 0)
            {
                NaturalAbilityTextBlock.Visibility = Visibility.Collapsed;
                NaturalAbilityStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                var thickness = new Thickness(20, 0, 0, 0);
                foreach (var str in c.Ability)
                {
                    if (str.Substring(0, 2) == "A_")
                    {
                        var stackPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = thickness
                        };
                        var textBlock = new TextBlock
                        {
                            Text = "生成",
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        var picButton = new PicButton
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Margin = new Thickness(5, 0, 0, 0),
                            Source = StringProcess.GetGameResourcePath(str)
                        };
                        stackPanel.Children.Add(textBlock);
                        stackPanel.Children.Add(picButton);
                        NaturalAbilityStackPanel.Children.Add(stackPanel);
                    }
                    else
                    {
                        var textBlock = new TextBlock
                        {
                            Text = str,
                            Margin = thickness
                        };
                        NaturalAbilityStackPanel.Children.Add(textBlock);
                    }
                    //picButton.Click += Creature_Jump_Click;
                }
            }
            // 主要生物群落
            var biomesThickness = new Thickness(2, 0, 2, 0);
            foreach (var str in c.Biomes)
            {
                var picButton = new PicButton
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = biomesThickness,
                    Source = StringProcess.GetGameResourcePath(str),
                    PictureSize = 90
                };
                //picButton.Click += Creature_Jump_Click;
                NaturalBiomesWrapPanel.Children.Add(picButton);
            }
            // 介绍
            NatureIntroduction.Text = c.Introduction;
            // 控制台
            if (c.EnName.Contains("Diseased"))
            {
                CopyGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                ConsolePre.Text = $"c_spawn(\"{c.Console}\",";
            }
        }

        /// <summary>
        /// 显示资源
        /// </summary>
        /// <param name="index">索引序号</param>
        private void ShowResources(int index)
        {
            NaturalResourceStackPanel.Children.Clear();
            var thickness = new Thickness(20, 0, 0, 0);
            if (_smallPlantResourceListStringList[index].Count == 0)
            {
                NaturalResourceTextBlock.Visibility = Visibility.Collapsed;
                NaturalResourceStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                NaturalResourceTextBlock.Visibility = Visibility.Visible;
                NaturalResourceStackPanel.Visibility = Visibility.Visible;
                foreach (var str in _smallPlantResourceListStringList[index])
                {
                    // 数量分割点
                    var breakPosition = str.IndexOf('|');
                    // 工具分割点
                    var toolBreakPosition = str.IndexOf('&');
                    // 图片
                    var resourceSource = str.Substring(0, breakPosition);
                    // 数量文本
                    var resourceText = str.Substring(breakPosition + 1, toolBreakPosition - breakPosition - 1);
                    // 工具
                    var toolText = str.Substring(toolBreakPosition + 1);
                    // 使用工具
                    if (toolText.Length > 0)
                    {
                        var stackPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = thickness
                        };
                        var picButton1 = new PicButton
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Source = StringProcess.GetGameResourcePath(resourceSource),
                            Text = resourceText + "（"
                        };
                        stackPanel.Children.Add(picButton1);
                        PicButton picButton2;
                        PicButton picButton3;
                        if (toolText == "Shovel")
                        {
                            picButton2 = new PicButton
                            {
                                Source = StringProcess.GetGameResourcePath("S_shovel")
                            };
                            picButton3 = new PicButton
                            {
                                Source = StringProcess.GetGameResourcePath("S_goldenshovel"),
                                Text = "）"
                            };
                            stackPanel.Children.Add(picButton2);
                            stackPanel.Children.Add(picButton3);
                        }
                        else if (toolText == "Machete")
                        {
                            picButton2 = new PicButton
                            {
                                Source = StringProcess.GetGameResourcePath("S_machete")
                            };
                            picButton3 = new PicButton
                            {
                                Source = StringProcess.GetGameResourcePath("S_luxury_machete"),
                            };
                            var picButton4 = new PicButton
                            {
                                Source = StringProcess.GetGameResourcePath("S_obsidian_machete"),
                                Text = "）"
                            };
                            stackPanel.Children.Add(picButton2);
                            stackPanel.Children.Add(picButton3);
                            stackPanel.Children.Add(picButton4);
                        }
                        else if (toolText == "TrawlNet")
                        {
                            picButton2 = new PicButton
                            {
                                Source = StringProcess.GetGameResourcePath("S_trawl_net"),
                                Text = "）"
                            };
                            stackPanel.Children.Add(picButton2);
                        }
                        //picButton1.Click += Creature_Jump_Click;
                        //picButton2.Click += Creature_Jump_Click;
                        NaturalResourceStackPanel.Children.Add(stackPanel);
                    }
                    else
                    {
                        var picButton = new PicButton
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Margin = thickness,
                            Source = StringProcess.GetGameResourcePath(resourceSource),
                            Text = resourceText
                        };
                        //picButton.Click += Creature_Jump_Click;
                        NaturalResourceStackPanel.Children.Add(picButton);
                    }
                }
            }
        }

        /// <summary>
        /// 左右切换按钮
        /// </summary>
        private void SwitchLeftButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SwitchRightButton.IsEnabled = true;
            if (_smallPlantIndex != 0)
            {
                _smallPlantIndex -= 1;
                if (_smallPlantIndex == 0)
                {
                    SwitchLeftButton.IsEnabled = false;
                }
                NatureImage.Source = new BitmapImage(new Uri(_smallPlantList[_smallPlantIndex]));
                ShowResources(_smallPlantIndex);
            }
        }

        private void SwitchRightButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SwitchLeftButton.IsEnabled = true;
            if (_smallPlantIndex != _smallPlantMaxIndex)
            {
                _smallPlantIndex += 1;
                if (_smallPlantIndex == _smallPlantMaxIndex)
                {
                    SwitchRightButton.IsEnabled = false;
                }
                NatureImage.Source = new BitmapImage(new Uri(_smallPlantList[_smallPlantIndex]));
                ShowResources(_smallPlantIndex);
            }
        }

        private void ConsoleNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = (TextBox)sender;
            StringProcess.ConsoleNumTextCheck(textbox);
        }

        private void Copy_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            if (string.IsNullOrEmpty(ConsoleNum.Text) || double.Parse(ConsoleNum.Text) == 0)
            {
                ConsoleNum.Text = "1";
            }
            dataPackage.SetText(ConsolePre.Text + ConsoleNum.Text + ")");
            Clipboard.SetContent(dataPackage);
        }

        private void ScrollViewer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var list = new List<DependencyObject>();
            Global.FindChildren(list, (ScrollViewer)sender);
            var scrollViewerGrid = (from dependencyObject in list where dependencyObject.ToString() == "Windows.UI.Xaml.Controls.Grid" select dependencyObject.GetHashCode()).FirstOrDefault();
            if (e.OriginalSource.GetHashCode() == scrollViewerGrid)
            {
                Global.App_BackRequested();
            }
        }
    }
}
