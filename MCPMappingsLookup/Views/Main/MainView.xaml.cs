﻿using System.Windows;
using TheRFramework.Utilities;

namespace MCPMappingsLookup.Views.Main
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window, BaseView<MainViewModel>
    {
        public MainViewModel Model
        {
            get => this.DataContext as MainViewModel;
            set => this.DataContext = value;
        }

        public MainView()
        {
            InitializeComponent();
        }
    }
}
