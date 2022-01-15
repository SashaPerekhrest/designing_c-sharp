namespace Inheritance.MapObjects
{ 
    public interface IMapObject
    {
        void Interact(Player p);
    }

    public interface IArmy
    {
        Army Army { get; set; }
    }

    public interface IOwner
    {
        int Owner { get; set; }
    }

    public interface ITreasure
    {
        Treasure Treasure { get; set; }
    }

    public class FightArmy
    {
        public static bool Fight(Player p, Army army)
        {
            if (p.CanBeat(army))
                return true;
            p.Die();
            return false;
        }
    }

    public class Dwelling : IMapObject
    {
        public int Owner { get; set; }
        public void Interact(Player p) => Owner = p.Id;
    }

    public class Mine : IMapObject, IOwner, IArmy
    {
        public int Owner { get; set; }
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
        public void Interact(Player p)
        {
            if (FightArmy.Fight(p, Army))
            {
                Owner = p.Id;
                p.Consume(Treasure);
            }
        }
    }

    public class Creeps : IMapObject, IArmy
    {
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }

        public void Interact(Player p)
        {
            if (FightArmy.Fight(p, Army))
            {
                p.Consume(Treasure);
            }
        }
    }

    public class Wolves : IMapObject
    {
        public void Interact(Player p) => p.Die();
    }

    public class ResourcePile : IMapObject
    {
        public Treasure Treasure { get; set; }
        public void Interact(Player p) => p.Consume(Treasure);     
    }

    public static class Interaction
    {
        public static void Make(Player player, IMapObject obj) => obj.Interact(player);
    }
}