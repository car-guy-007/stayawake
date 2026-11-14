namespace StayAwake;

public sealed class MainForm : Form
{
    private readonly ComboBox _duration = new();
    private readonly CheckBox _display = new();
    private readonly Button _toggle = new();
    private readonly Label _status = new();
    private readonly Label _remaining = new();
    private readonly NotifyIcon _tray = new();
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 1000 };
    private DateTime? _endAt;
    private bool _active;

    public MainForm()
    {
        Text = "StayAwake";
        MinimumSize = new Size(400, 310);
        Size = new Size(420, 310);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var heading = new Label { Text = "StayAwake", Font = new Font("Segoe UI", 19, FontStyle.Bold), AutoSize = true, Location = new Point(22, 18) };
        var subtitle = new Label { Text = "Keep Windows awake without admin privileges.", AutoSize = true, ForeColor = Color.DimGray, Location = new Point(25, 58) };
        var durationLabel = new Label { Text = "Keep awake for", AutoSize = true, Location = new Point(25, 103) };
        _duration.DropDownStyle = ComboBoxStyle.DropDownList;
        _duration.Items.AddRange(new object[] { "Indefinitely", "30 minutes", "1 hour", "2 hours", "4 hours", "8 hours" });
        _duration.SelectedIndex = 0;
        _duration.Location = new Point(25, 126);
        _duration.Width = 345;
        _display.Text = "Keep the display on too";
        _display.AutoSize = true;
        _display.Location = new Point(25, 165);
        _toggle.Text = "Start keeping awake";
        _toggle.Location = new Point(25, 198);
        _toggle.Size = new Size(345, 38);
        _toggle.Click += (_, _) => ToggleAwake();
        _status.Text = "● Inactive";
        _status.AutoSize = true;
        _status.Location = new Point(25, 247);
        _status.ForeColor = Color.DimGray;
        _remaining.AutoSize = true;
        _remaining.Location = new Point(150, 247);
        _remaining.ForeColor = Color.DimGray;
        Controls.AddRange(new Control[] { heading, subtitle, durationLabel, _duration, _display, _toggle, _status, _remaining });

        var menu = new ContextMenuStrip();
        menu.Items.Add("Show StayAwake", null, (_, _) => ShowFromTray());
        menu.Items.Add("Stop keeping awake", null, (_, _) => StopAwake());
        menu.Items.Add("Exit", null, (_, _) => Close());
        _tray.Icon = SystemIcons.Application;
        _tray.Text = "StayAwake";
        _tray.ContextMenuStrip = menu;
        _tray.Visible = true;
        _tray.DoubleClick += (_, _) => ShowFromTray();
        _timer.Tick += (_, _) => UpdateCountdown();
        FormClosing += (_, _) => { _timer.Stop(); StopAwake(); _tray.Visible = false; _tray.Dispose(); };
    }

    private void ToggleAwake()
    {
        if (_active) { StopAwake(); return; }
        if (!ExecutionState.KeepAwake(_display.Checked))
        {
            MessageBox.Show(this, "Windows did not accept the keep-awake request. No system settings were changed.", "StayAwake", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        _active = true;
        _endAt = _duration.SelectedIndex switch
        {
            1 => DateTime.Now.AddMinutes(30), 2 => DateTime.Now.AddHours(1),
            3 => DateTime.Now.AddHours(2), 4 => DateTime.Now.AddHours(4),
            5 => DateTime.Now.AddHours(8), _ => null
        };
        _duration.Enabled = false;
        _display.Enabled = false;
        _toggle.Text = "Stop keeping awake";
        _status.Text = "● Active";
        _status.ForeColor = Color.ForestGreen;
        _tray.Text = "StayAwake - active";
        _timer.Start();
        UpdateCountdown();
    }

    private void StopAwake()
    {
        if (!_active) return;
        _timer.Stop();
        ExecutionState.RestoreDefault();
        _active = false;
        _endAt = null;
        _duration.Enabled = true;
        _display.Enabled = true;
        _toggle.Text = "Start keeping awake";
        _status.Text = "● Inactive";
        _status.ForeColor = Color.DimGray;
        _remaining.Text = "";
        _tray.Text = "StayAwake - inactive";
    }

    private void UpdateCountdown()
    {
        if (!_active) return;
        if (_endAt is null) { _remaining.Text = "No time limit"; return; }
        var left = _endAt.Value - DateTime.Now;
        if (left <= TimeSpan.Zero) { StopAwake(); return; }
        _remaining.Text = $"{(int)left.TotalHours:00}:{left.Minutes:00}:{left.Seconds:00} remaining";
    }

    private void ShowFromTray()
    {
        Show();
        WindowState = FormWindowState.Normal;
        Activate();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (WindowState == FormWindowState.Minimized) Hide();
    }
}
