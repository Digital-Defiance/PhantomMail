using Terminal.Gui;

namespace PhantomKit.Models.Views;

internal class Box10X : View
{
    private readonly int h = 50;
    private readonly int w = 40;

    public Box10X(int x, int y) : base(frame: new Rect(x: x,
        y: y,
        width: 20,
        height: 10))
    {
    }

    public bool WantCursorPosition { get; set; } = false;

    public Size GetContentSize()
    {
        return new Size(width: this.w,
            height: this.h);
    }

    public void SetCursorPosition(Point pos)
    {
        throw new NotImplementedException();
    }

    public override void Redraw(Rect bounds)
    {
        //Point pos = new Point (region.X, region.Y);
        Driver.SetAttribute(c: this.ColorScheme.Focus);

        for (var y = 0; y < this.h; y++)
        {
            this.Move(col: 0,
                row: y);
            Driver.AddStr(str: y.ToString());
            for (var x = 0; x < this.w - y.ToString().Length; x++) //Driver.AddRune ((Rune)('0' + (x + y) % 10));
                if (y.ToString().Length < this.w)
                    Driver.AddStr(str: " ");
        }
        //Move (pos.X, pos.Y);
    }
}