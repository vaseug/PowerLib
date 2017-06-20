using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerLib.System.Numerics
{
  using Math = global::System.Math;

  public static class Angle
  {
    private const double TotalDegrees = 360d;
    private const double TotalGrads = 400d;
    private const double TotalHours = 24d;
    private const double TotalRadians = Math.PI * 2d;

    public static double NormDegree(double degrees)
    {
      return degrees > -TotalDegrees && degrees < TotalDegrees ? degrees : degrees - Math.Truncate(degrees / TotalDegrees) * TotalDegrees;
    }

    public static double NormGrad(double grads)
    {
      return grads > -TotalGrads && grads < TotalGrads ? grads : grads - Math.Truncate(grads / TotalGrads) * TotalGrads;
    }

    public static double NormHour(double hours)
    {
      return hours > -TotalHours && hours < TotalHours ? hours : hours - Math.Truncate(hours / TotalHours) * TotalHours;
    }

    public static double NormRadian(double radians)
    {
      return radians > -TotalRadians && radians < TotalRadians ? radians : radians - Math.Truncate(radians / TotalRadians) * TotalRadians;
    }

    public static double FromDegree(double degrees)
    {
      return NormDegree(degrees) * TotalRadians / TotalDegrees;
    }

    public static double FromGrad(double grads)
    {
      return NormGrad(grads) * TotalRadians / TotalGrads;
    }

    public static double FromHour(double hours)
    {
      return NormHour(hours) * TotalRadians / TotalHours;
    }

    public static double ToDegree(double radians)
    {
      return NormRadian(radians) * TotalDegrees / TotalRadians;
    }

    public static double ToGrad(double radians)
    {
      return NormRadian(radians) * TotalGrads / TotalRadians;
    }

    public static double ToHour(double radians)
    {
      return NormRadian(radians) * TotalHours / TotalRadians; ;
    }

    public static double Normalize(double units, double totalUnits)
    {
      return units > -totalUnits && units < totalUnits ? units : units - Math.Truncate(units / totalUnits) * totalUnits;
    }

    public static double ToRadian(double units, double totalUnits)
    {
      return Normalize(units, totalUnits) * TotalRadians / totalUnits;
    }

    public static double FromRadian(double radian, double totalUnits)
    {
      return NormRadian(radian) * totalUnits / TotalRadians;
    }
  }
}
