using System.Windows.Input;

namespace GraphBuilder
{
    public static class Commands
    {
        public static readonly RoutedCommand CreateNew = new RoutedCommand("CreateNew", typeof(Commands));
        public static readonly RoutedCommand Open = new RoutedCommand("Open", typeof(Commands));
        public static readonly RoutedCommand Save = new RoutedCommand("Save", typeof(Commands));
        public static readonly RoutedCommand Copy = new RoutedCommand("Copy", typeof(Commands));
        public static readonly RoutedCommand Paste = new RoutedCommand("Paste", typeof(Commands));
        public static readonly RoutedCommand InvertX = new RoutedCommand("InvertX", typeof(Commands));
        public static readonly RoutedCommand InvertY = new RoutedCommand("InvertY", typeof(Commands));
        public static readonly RoutedCommand ExportSvg = new RoutedCommand("ExportSvg", typeof(Commands));
        public static readonly RoutedCommand About = new RoutedCommand("About", typeof(Commands));

        static Commands()
        {
            CreateNew.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            Open.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            Save.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            Copy.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));
            Paste.InputGestures.Add(new KeyGesture(Key.V, ModifierKeys.Control));
            InvertX.InputGestures.Add(new KeyGesture(Key.X, ModifierKeys.Control));
            InvertY.InputGestures.Add(new KeyGesture(Key.Y, ModifierKeys.Control));
            ExportSvg.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Control));
            About.InputGestures.Add(new KeyGesture(Key.F1));
        }
    }
}
