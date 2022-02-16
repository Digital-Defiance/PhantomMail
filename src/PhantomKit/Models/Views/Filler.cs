using Terminal.Gui;

namespace PhantomKit.Models.Views;

internal class Filler : View
{
    private int h = 50;
    private int w = 40;

    public Filler(Rect rect) : base(frame: rect)
    {
        this.w = rect.Width;
        this.h = rect.Height;
    }

    public Size GetContentSize()
    {
        return new Size(width: this.w,
            height: this.h);
    }

    public override void Redraw(Rect bounds)
    {
        Driver.SetAttribute(c: this.ColorScheme.Focus);
        var f = this.Frame;
        this.w = 0;
        this.h = 0;

        for (var y = 0; y < f.Width; y++)
        {
            this.Move(col: 0,
                row: y);
            var nw = 0;
            for (var x = 0; x < f.Height; x++)
            {
                Rune r;
                switch (x % 3)
                {
                    case 0:
                        var er = y.ToString().ToCharArray(startIndex: 0,
                            length: 1)[0];
                        nw += er.ToString().Length;
                        Driver.AddRune(rune: er);
                        if (y > 9)
                        {
                            er = y.ToString().ToCharArray(startIndex: 1,
                                length: 1)[0];
                            nw += er.ToString().Length;
                            Driver.AddRune(rune: er);
                        }

                        r = '.';
                        break;
                    case 1:
                        r = 'o';
                        break;
                    default:
                        r = 'O';
                        break;
                }

                Driver.AddRune(rune: r);
                nw += Rune.RuneLen(rune: r);
            }

            if (nw > this.w)
                this.w = nw;
            this.h = y + 1;
        }
    }
}