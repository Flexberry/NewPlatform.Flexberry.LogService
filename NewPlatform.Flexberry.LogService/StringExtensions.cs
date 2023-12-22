using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSSoft.STORMNET
{
    /// <summary>
    /// Расширения для работы со строками.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Вернуть null если строка пустая (нужен для null-coalescing ?? оператора).
        /// </summary>
        /// <param name="s">Проверяемая строка.</param>
        /// <returns>null если строка пустая.</returns>
        public static string NullIfEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) ? null : s;
        }
    }
}
