﻿namespace ePicSearch.Infrastructure.Entities
{
    public class Settings
    {
        public bool EnableNotifications { get; set; } = true;
        public bool ShowTutorials { get; set; } = true;

        public bool PlayStartupVideo { get; set; } = true;

        public int MaxPhotoCount { get; set; } = 100;

        public bool IsMuted { get; set; } = false;

        public bool CrashFlag { get; set; } = false;
    }
}