namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public partial class MyStrategy
    {
        public bool EvadeNuclearStrike()
        {
            var enemy = Global.World.GetOpponentPlayer();

            if (Global.World.TickIndex == enemy.NextNuclearStrikeTickIndex - 29)
            {
                Actions.SelectAll();
                Actions.Scale(10, enemy.NextNuclearStrikeX, enemy.NextNuclearStrikeY);

                return true;
            }

            return false;
        }
    }
}