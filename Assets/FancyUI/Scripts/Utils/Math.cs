using UnityEngine;


namespace Utils.Math
{
    public class Math
    {
        public static double Normalize(double duration, double elapsed)
        {
            double normalizedT = (duration == 0) ? 1.0f : elapsed / duration;
            if (normalizedT > 1)
            {
                normalizedT = 1.0f;
            }
            return normalizedT;
        }
    }
}