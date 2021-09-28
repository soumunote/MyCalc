using System;
using System.Diagnostics;
using System.Text;

class SafeDecimal
{
    public static readonly int MAX_ABS_SIGNIFICANT = 99999999;
    public static readonly int RADIX = 10; // 10進数以外でも応用可能？

    private int m_sign;
    private long m_significant;
    private int m_scale; // 12,000 を 12E03 と表現するなら、マイナスも許すべき

    public int Sign => m_sign;
    public long Significant => m_significant;
    public int Scale => m_scale;

    private SafeDecimal(int p_sign, long p_significant, int p_scale)
    {
        m_sign = p_sign;
        m_significant = p_significant;
        m_scale = p_scale;
    }

    /// <summary>
    /// 文字列からオブジェクトを生成する
    /// </summary>
    /// <param name="p_value"></param>
    /// <remarks>エラーチェック等、あまりにもいいかげんです</remarks>
    public SafeDecimal(string p_value)
    {
        m_sign = 1; // TODO 今は、正数だけ
        m_significant = long.Parse(p_value.Replace(".", null));
        int posOfPoint = p_value.LastIndexOf('.');
        m_scale = posOfPoint == -1 ? 0
                                   : p_value.Length - posOfPoint + 1; 
    }

    public override string ToString()
    {
        var value = new StringBuilder();
        long significant = m_significant;
        int digit;
        int currentPos = 0;
        do
        {
            digit = (int)(significant % 10);
            significant /= RADIX;

            value.Append((char)(digit + '0'));
            currentPos++;

            if (currentPos == m_scale) value.Append('.');

        } while (!(significant == 0 && currentPos > m_scale));

        var ansAsAry = value.ToString().ToCharArray();
        Array.Reverse(ansAsAry);
        return new string(ansAsAry);
    }

    public static SafeDecimal operator / (SafeDecimal p_lhs, SafeDecimal p_rhs)
    {
        if (p_rhs.m_significant == 0) throw new DivideByZeroException();

        var ans = 0L;
        var scale = p_lhs.Scale - p_rhs.Scale;
        var lhs = p_lhs.m_significant;
          var rhs = p_rhs.m_significant;

        if (lhs == 0) return new SafeDecimal(0, 0, 0);

        while (lhs != 0 && ans <= MAX_ABS_SIGNIFICANT)
        {
            // 1桁分の処理(最初の1発目...被除数 >> 除数 の場合、1桁以上の場合あり。e.g. 1023/12 = 85...)
            long ansOfCurrentDigit = lhs / rhs;
            long remainder         = lhs % rhs;

            // 商の組み立て && 次の商の作成
            Debug.Assert(ans == 0 || ansOfCurrentDigit < 10, "最初の除算以外は、一次的な商は1桁のはず");
            ans = ans * RADIX　+ ansOfCurrentDigit;
            lhs = remainder * RADIX;
            scale ++;
        }

        return new SafeDecimal(p_lhs.m_sign * p_rhs.m_sign, ans, scale - 1);
    }
}
