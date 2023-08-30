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
		/// <summary>
		/// Converts the given <see cref="double"/> to a percentage-based value.<br/>
		/// Mainly for use in tooltips.<br/>
		/// </summary>
		/// <param name="doubleToConvert">
		/// The <see cref="double"/> to be converted into a percentage-based value.
		/// </param>
		/// <param name="maxDecimalPlaces">
		/// The maximum number of decimal places to allow for the converted percentage.
		/// </param>
		/// <returns>
		/// The provided <see cref="double"/>, as a percentage-based value.
		/// </returns>
		public static string ToPercentage(this double doubleToConvert, int maxDecimalPlaces = 0) => CastToDecimalPlaces(doubleToConvert * 100.0, maxDecimalPlaces) + "%";
		/// <summary>
		/// Converts the given <see cref="float"/> to a percentage-based value.<br/>
		/// Mainly for use in tooltips.<br/>
		/// </summary>
		/// <param name="floatToConvert">
		/// The <see cref="float"/> to be converted into a percentage-based value.
		/// </param>
		/// <param name="maxDecimalPlaces">
		/// The maximum number of decimal places to allow for the converted percentage.
		/// </param>
		/// <returns>
		/// The provided <see cref="float"/>, as a percentage-based value.
		/// </returns>
		public static string ToPercentage(this float floatToConvert, int maxDecimalPlaces = 0) => CastToDecimalPlaces(floatToConvert * 100f, maxDecimalPlaces) + "%";

	}
}
