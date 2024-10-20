﻿using ePicSearch.Infrastructure.Services;

namespace ePicSearch
{
    public partial class App : Application
    {
        private readonly DataStorageService _dataStorageService;
        public App(AppShell appShell, DataStorageService jsonStorageService)
        {
            InitializeComponent();
            MainPage = appShell;
            _dataStorageService = jsonStorageService;
        }

        // Sync cache when the app goes to sleep (background or closing)
        protected override void OnSleep()
        {
            _dataStorageService.SyncCacheToFile();
            base.OnSleep();
        }
    }
}

