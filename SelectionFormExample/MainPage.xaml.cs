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

	/// <summary>
	/// The counter
	/// </summary>
	public int Counter { get; set; }

	/// <summary>
	/// Mouse X coordinate relative to the page (not the screen coordinates)
	/// </summary>
	public int MouseX { get; set; }
	/// <summary>
	/// Mouse X coordinate relative to the page (not the screen coordinates)
	/// </summary>
	public int MouseY { get; set; }

	/// <summary>
	/// Width of the window
	/// </summary>
	public int WindowWidth { get; set; }

	/// <summary>
	/// Height of the window
	/// </summary>
	public int WindowHeight { get; set; }

	/// <summary>
	/// System.Reactive observable subscription used for the mouse events
	/// </summary>
	IDisposable subscription;

	public MainPage()
	{
		InitializeComponent();
		// Initialize the WinProc handler, this is something that needs to be available before we can use Dapplo.Windows to handle mouse or keyboard (and clipboard etc)
		_ = WinProcHandler.Instance;
	}

	/// <summary>
	/// Use the on appearing to subscribe to mouse events
	/// </summary>
	protected override void OnAppearing()
	{
		base.OnAppearing();
		// Subscribe (System.Reactive) to all mouse events, filter onto for the move events, handle these in HandleMouseMove
		subscription = MouseHook.MouseEvents.Where(args => args.WindowsMessage == WindowsMessages.WM_MOUSEMOVE).Subscribe(mheA => HandleMouseMove(mheA));
	}

	/// <summary>
	/// Use the on disappearing to unsubscribe to mouse events
	/// </summary>
	protected override void OnDisappearing()
	{
		subscription?.Dispose();
		base.OnDisappearing();

	}

	/// <summary>
	/// Handle the mouse move
	/// </summary>
	/// <param name="mouseHookEventArgs">MouseHookEventArgs with the mouse information</param>
	private void HandleMouseMove(MouseHookEventArgs mouseHookEventArgs)
    {
		if (!this.IsVisible)
		{
			return;
		}
		WindowInfo thisWindow;

		// If we didn't have the window information yet, like the bounds, get it.
		// Actually it wouldn't hurt to do this all the time, as the window can move, but there are better ways to do so.
		if (_hWnd == IntPtr.Zero)
        {
			// Get the hWND for this window
			_hWnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;
			// Get the information, via Win32 interop.
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
		// Calculate the relative mouse coordinates in the page, this is actually not correct as I left out the titlebar and the mouse "hotspot" so far.
		MouseX = mouseHookEventArgs.Point.X - thisWindow.Bounds.X;
		MouseY = mouseHookEventArgs.Point.Y - thisWindow.Bounds.Y;

		// Inform the bindings that the values have changed.
		OnPropertyChanged(nameof(MouseX));
		OnPropertyChanged(nameof(MouseY));
	}

	/// <summary>
	/// Handle the counter button clicked
	/// </summary>
	/// <param name="sender">object</param>
	/// <param name="e">EventArgs</param>
	private void OnCounterClicked(object sender, EventArgs e)
	{
		Counter++;
		OnPropertyChanged(nameof(Counter));
	}
}

