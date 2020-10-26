namespace Mis1eader.MediaPlayer
{
	public static class Library
	{
		public static float RangeConversion (float value,float minimumValue,float maximumValue,float minimum,float maximum) {return minimumValue != maximumValue ? minimum + (value - minimumValue) / (maximumValue - minimumValue) * (maximum - minimum) : minimum;}
	}
}