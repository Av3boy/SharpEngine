using Xunit;
using System.Numerics;
using SharpEngine.Text;

public class GeometryUtilsTests
{
    /*[Fact]
    public void EvaluateQuadratic_MidpointIsCorrect()
    {
        var p0 = new Vector2(0, 0);
        var p1 = new Vector2(0, 2);
        var p2 = new Vector2(2, 2);

        var mid = GeometryUtils.EvaluateQuadratic(p0, p1, p2, 0.5f);

        Assert.Equal(0.5f, mid.X, 3);
        Assert.Equal(1.5f, mid.Y, 3);
    }

    [Fact]
    public void EvaluateCubic_Endpoints()
    {
        var p0 = new Vector2(0, 0);
        var p1 = new Vector2(0, 1);
        var p2 = new Vector2(1, 2);
        var p3 = new Vector2(2, 2);

        var start = GeometryUtils.EvaluateCubic(p0, p1, p2, p3, 0f);
        var end = GeometryUtils.EvaluateCubic(p0, p1, p2, p3, 1f);

        Assert.Equal(p0.X, start.X);
        Assert.Equal(p0.Y, start.Y);
        Assert.Equal(p3.X, end.X);
        Assert.Equal(p3.Y, end.Y);
    }

    [Fact]
    public void IsPointInPolygon_Square()
    {
        var poly = new System.Collections.Generic.List<Vector2>
        {
            new Vector2(0,0), new Vector2(2,0), new Vector2(2,2), new Vector2(0,2)
        };

        Assert.True(GeometryUtils.IsPointInPolygon(poly, new Vector2(1,1)));
        Assert.False(GeometryUtils.IsPointInPolygon(poly, new Vector2(3,3)));
    }*/
}
