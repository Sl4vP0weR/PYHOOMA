namespace SomeGame
{
    public struct Border
    {
        public char LT, LB, RT, RB, T, B, L, R;

        public Border(char lT, char lB, char rT, char rB, char t, char b, char l, char r)
        {
            LT = lT; LB = lB; RT = rT; RB = rB; T = t; B = b; L = l; R = r;
        }
    }
}
