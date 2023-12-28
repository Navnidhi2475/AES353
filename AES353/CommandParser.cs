using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

public class CommandParser
{
    private TextBox codeTextBox;
    private PictureBox displayArea;
    private Graphics graphics;
    private Pen currentPen;
    private PointF currentPosition;
    private bool fillEnabled = false;

    public CommandParser(TextBox codeTextBox, PictureBox displayArea)
    {
        this.codeTextBox = codeTextBox;
        this.displayArea = displayArea;
        this.graphics = displayArea.CreateGraphics();
        this.currentPen = new Pen(Color.Black);
        this.currentPosition = new PointF(0, 0);
    }

    public void ExecuteCommand(string command)
    {
        var parts = command.Split(' ');
        switch (parts[0].ToLower())
        {
            case "moveto":
                MoveTo(float.Parse(parts[1]), float.Parse(parts[2]));
                break;
            case "drawto":
                DrawTo(float.Parse(parts[1]), float.Parse(parts[2]));
                break;
            case "clear":
                Clear();
                break;
            case "rectangle":
                DrawRectangle(float.Parse(parts[1]), float.Parse(parts[2]));
                break;
            case "circle":
                DrawCircle(float.Parse(parts[1]));
                break;
            case "triangle":
                DrawTriangle(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]));
                break;
            case "color":
                SetColor(Color.FromName(parts[1]));
                break;
            case "reset":
                ResetPenPosition();
                break;
            case "fill":
                ToggleFill(parts[1]);
                break;
            default:
                throw new ArgumentException("Unknown command");
        }
    }

    public void ExecuteProgram()
    {
        var commands = codeTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var command in commands)
        {
            ExecuteCommand(command.Trim());
        }
    }

    public void SaveProgram(string filePath)
    {
        File.WriteAllText(filePath, codeTextBox.Text);
    }

    public void LoadProgram(string filePath)
    {
        codeTextBox.Text = File.ReadAllText(filePath);
    }

    public void CheckSyntax()
    {
        var commands = codeTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var command in commands)
        {
            if (!IsValidCommand(command))
            {
                MessageBox.Show($"Syntax error in command: {command}", "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        MessageBox.Show("Syntax is correct!", "Syntax Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private bool IsValidCommand(string command)
    {
        var parts = command.Split(' ');
        var commandType = parts[0].ToLower();

        switch (commandType)
        {
            case "moveto":
            case "drawto":
                return parts.Length == 3 && parts.Skip(1).All(p => float.TryParse(p, out _));
            case "rectangle":
                return parts.Length == 5 && parts.Skip(1).All(p => float.TryParse(p, out _));
            case "circle":
                return parts.Length == 2 && float.TryParse(parts[1], out _);
            case "triangle":
                return parts.Length == 7 && parts.Skip(1).All(p => float.TryParse(p, out _));
            case "color":
                return parts.Length == 2 && Enum.IsDefined(typeof(KnownColor), parts[1]);
            case "clear":
            case "reset":
                return parts.Length == 1;
            case "fill":
                return parts.Length == 2 && (parts[1].ToLower() == "on" || parts[1].ToLower() == "off");
            default:
                return false;
        }
    }

    private void MoveTo(float x, float y)
    {
        currentPosition = new PointF(x, y);
    }

    private void DrawTo(float x, float y)
    {
        PointF newPosition = new PointF(x, y);
        graphics.DrawLine(currentPen, currentPosition, newPosition);
        currentPosition = newPosition;
    }

    private void Clear()
    {
        graphics.Clear(displayArea.BackColor);
        currentPosition = new PointF(0, 0);
    }

    private void DrawRectangle(float width, float height)
    {
        if (fillEnabled)
            graphics.FillRectangle(currentPen.Brush, currentPosition.X, currentPosition.Y, width, height);
        else
            graphics.DrawRectangle(currentPen, currentPosition.X, currentPosition.Y, width, height);
    }

    private void DrawCircle(float radius)
    {
        if (fillEnabled)
            graphics.FillEllipse(currentPen.Brush, currentPosition.X - radius, currentPosition.Y - radius, radius * 2, radius * 2);
        else
            graphics.DrawEllipse(currentPen, currentPosition.X - radius, currentPosition.Y - radius, radius * 2, radius * 2);
    }

    private void DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        PointF[] points = { new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3) };
        if (fillEnabled)
            graphics.FillPolygon(currentPen.Brush, points);
        else
            graphics.DrawPolygon(currentPen, points);
    }

    private void SetColor(Color color)
    {
        currentPen.Color = color;
    }

    private void ResetPenPosition()
    {
        currentPosition = new PointF(0, 0);
    }

    private void ToggleFill(string state)
    {
        fillEnabled = state.ToLower() == "on";
    }

    public void SetupGraphics(PaintEventArgs e)
    {
        var g = e.Graphics;

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;


    }


    public void Cleanup()
    {
        if (graphics != null)
            graphics.Dispose();
        if (currentPen != null)
            currentPen.Dispose();
    }
}

