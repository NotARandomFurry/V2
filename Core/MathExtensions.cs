using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V2.Core
{
	public static class MathExtensions
	{
		public static double CastToDecimalPlaces(this double doubleToCast, int decimalPlaces) => Math.Round(doubleToCast * Math.Pow(10.0, (double)decimalPlaces)) / Math.Pow(10.0, (double)decimalPlaces);
		public static float CastToDecimalPlaces(this float floatToCast, int decimalPlaces) => (float)(Math.Round(floatToCast * Math.Pow(10.0, (double)decimalPlaces)) / Math.Pow(10.0, (double)decimalPlaces));
		public static string ConvertToPercentageString(this double doubleToConvert, int decimalPlaces = 0) => CastToDecimalPlaces(doubleToConvert * 100.0, decimalPlaces) + "%";
		public static string ConvertToPercentageString(this float floatToConvert, int decimalPlaces = 0) => CastToDecimalPlaces(floatToConvert * 100f, decimalPlaces) + "%";

	}
}
