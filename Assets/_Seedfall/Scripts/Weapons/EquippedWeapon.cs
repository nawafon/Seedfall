namespace Seedfall.Weapons
{
    // Per-instance durability wrapper. WeaponData is a shared asset (one Thornblaze
    // asset for every Thornblaze ever grafted) -- remaining hits can't live on it
    // without every copy of that weapon sharing one counter. WeaponInventory's slots
    // hold this instead of WeaponData directly.
    public class EquippedWeapon
    {
        public WeaponData Data { get; }
        public int RemainingHits { get; private set; }

        public EquippedWeapon(WeaponData data)
        {
            Data = data;
            RemainingHits = data.maxHits;
        }

        // Returns true if this hit wilted the weapon (durability hit 0).
        public bool DecrementAndCheckWilt()
        {
            RemainingHits--;
            return RemainingHits <= 0;
        }
    }
}
