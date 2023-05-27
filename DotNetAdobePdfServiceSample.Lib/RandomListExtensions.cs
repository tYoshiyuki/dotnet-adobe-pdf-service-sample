namespace DotNetAdobePdfServiceSample.Lib
{
    /// <summary>
    /// ランダムな要素を扱うための拡張メソッドです。
    /// </summary>
    public static class RandomListExtensions
    {
        private static readonly Random s_random = new();

        /// <summary>
        /// リストからランダムで要素を取得します。
        /// </summary>
        /// <typeparam name="T">対象となる型</typeparam>
        /// <param name="source"><see cref="IList{T}"/></param>
        /// <returns>取得した要素</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T Random<T>(this IList<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            return source.ElementAt(s_random.Next(source.Count));
        }
    }
}
