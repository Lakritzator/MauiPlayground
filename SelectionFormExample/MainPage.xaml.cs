using Dapplo.Windows.Desktop;
using Dapplo.Windows.Input.Mouse;
using Dapplo.Windows.Messages;
using Dapplo.Windows.Messages.Enumerations;
using Dapplo.Windows.User32.Structs;
using System.Reactive.Linq;

namespace SelectionFormExample;

public partial class MainPage : ContentPage
{
	private IntPtr _hWnd;

	public int Counter { get; set; }

	public int MouseX { get; set; }
	public int MouseY { get; set; }

	public int WindowWidth { get; set; }
	public int WindowHeight { get; set; }

	IDisposable subscription;

	public MainPage()
	{
		InitializeComponent();


	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		var winProcHandler = WinProcHandler.Instance;
		subscription = MouseHook.MouseEvents.Where(args => args.WindowsMessage == WindowsMessages.WM_MOUSEMOVE).Subscribe(mheA => HandleMouseMove(mheA));
	}

	protected override void OnDisappearing()
	{
		subscription?.Dispose();
		base.OnDisappearing();

	}
    private void HandleMouseMove(MouseHookEventArgs mouseHookEventArgs)
    {
		if (!this.IsVisible)
		{
			return;
		}
		WindowInfo thisWindow;

		if (_hWnd == IntPtr.Zero)
        {
			_hWnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;
			thisWindow = InteropWindowFactory.CreateFor(_hWnd).GetInfo();
			WindowWidth = thisWindow.Bounds.Width;
			WindowHeight = thisWindow.Bounds.Height;
			OnPropertyChanged(nameof(WindowWidth));
			OnPropertyChanged(nameof(WindowHeight));
		}
		else
        {
			thisWindow = InteropWindowFactory.CreateFor(_hWnd).GetInfo();
		}
		MouseX = mouseHookEventArgs.Point.X - thisWindow.Bounds.X;
		MouseY = mouseHookEventArgs.Point.Y - thisWindow.Bounds.Y;
		OnPropertyChanged(nameof(MouseX));
		OnPropertyChanged(nameof(MouseY));
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		Counter++;
		OnPropertyChanged(nameof(Counter));
	}
}

