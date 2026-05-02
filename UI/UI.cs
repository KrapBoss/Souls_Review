namespace CustomUI
{
    public static class UI
    {

        public static ActivityUI activityUI
        {
            get; internal set;
        }

        //static Semi_StaticUI _semiStaticUI;
        public static Semi_StaticUI semiStaticUI
        {
            get; internal set;
        }

        //static StaticUI _staticUI;
        public static StaticUI staticUI
        {
            get; internal set;
        }

        public static TopUI topUI
        { get; internal set; }

        public static MobileControllerUI mobileControllerUI
        { get; internal set;}
    }
}

