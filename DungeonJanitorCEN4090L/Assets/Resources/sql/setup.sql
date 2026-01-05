CREATE TABLE Weapon (
    Name TEXT,
    Range INTEGER,
    AttackSpeed REAL,
    CritChance INTEGER,
    ItemID TEXT PRIMARY KEY
);

CREATE TABLE Spell (
    Name TEXT,
    Range INTEGER,
    CastTime REAL,
    ManaCost INTEGER,
    DamageType INTEGER,
    ItemID TEXT PRIMARY KEY
);

CREATE TABLE Armor (
    Name TEXT,
    BlockChance INTEGER,
    DodgeChance INTEGER,
    FlatBlock INTEGER,
    ItemID TEXT PRIMARY KEY
);

CREATE TABLE Enemy (
    Name TEXT,
    BlockChance INTEGER,
    DodgeChance INTEGER,
    FlatBlock INTEGER,
    Resistance INTEGER,
    Health INTEGER,
    EnemyID TEXT PRIMARY KEY
);
