using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public static class ColorMobs
    {
        public static readonly Color Common = Color.Gray;
        public static readonly Color Brutal = Color.DarkOrange;
        public static readonly Color Elite = Color.MediumVioletRed;
        public static readonly Color Demonic = Color.Indigo;
        public static readonly Color Boss = Color.Red;
        public static readonly Color ArchBoss = Color.FromArgb(80, 0, 0);
    }

    public static class CloseCombatMobs
    {
        private static readonly Random random = new Random();

        public static CloseCombatMobsDifinition GetRandom()
        {
            CloseCombatMobsDifinition[] all =
            {
            Orc_Scavenger,
            Orc_Glutton,
            Orc_Raider,
            Orc_Crusher,
            Orc_Berserker,
            Orc_Veteran,
            Orc_Twisted,
            Orc_Brute,
            Orc_Warchief,
            Orc_CorruptedChief,
            Orc_BloodOverlord
        };

            return all[random.Next(all.Length)];
        }

        public static CloseCombatMobsDifinition GetMob1Act()
        {
            CloseCombatMobsDifinition[] low =
            {
            Orc_Scavenger,
            Orc_Glutton,
            Orc_Raider,
            Orc_Crusher
        };

            return low[random.Next(low.Length)];
        }
        public static CloseCombatMobsDifinition GetMob2Act()
        {
            CloseCombatMobsDifinition[] elite =
            {
            Orc_Berserker,
            Orc_Veteran,
            Orc_Brute,
        };

            return elite[random.Next(elite.Length)];
        }

        public static CloseCombatMobsDifinition GetMob3Act()
        {
            CloseCombatMobsDifinition[] elite =
            {
            Orc_Twisted,
            Orc_Potroshitel,
            Orc_Dark,
        };

            return elite[random.Next(elite.Length)];
        }

        public static CloseCombatMobsDifinition GetBoss()
        {
            CloseCombatMobsDifinition[] bosses =
            {
            Orc_Warchief,
            Orc_CorruptedChief,
            Orc_BloodOverlord
        };

            return bosses[random.Next(bosses.Length)];
        }




        public static readonly CloseCombatMobsDifinition Orc_Scavenger =
        new CloseCombatMobsDifinition("Орк-падальщик")
        {
            Description = "Слабый орк, выживающий за счёт остатков после битв. Опасен только в группе.",
            damage = 4,
            speed = 4,
            hp = 40,
            color = ColorMobs.Common,
        };

        public static readonly CloseCombatMobsDifinition Orc_Glutton =
        new CloseCombatMobsDifinition("Сын фермера")
        {
            Description = "Огромный и медлительный орк, привыкший давить врагов массой и выносливостью.",
            damage = 3,
            hp = 120,
            ChanceMiss = 30,
            speed = 2,
            color = ColorMobs.Elite,
            SpeedAttack = 50,
            DistanceAttack = 100,
        };

        public static readonly CloseCombatMobsDifinition Orc_Raider =
        new CloseCombatMobsDifinition("Орк-налётчик")
        {
            Description = "Быстрый и агрессивный орк, предпочитающий внезапные атаки и отступления.",
            damage = 3,
            speed = 10,
            ChanceMiss = 30,
            hp = 30,
            color = ColorMobs.Brutal,
            SpeedAttack = 10,
            DistanceAttack = 50,
        };

        public static readonly CloseCombatMobsDifinition Orc_Crusher =
        new CloseCombatMobsDifinition("Орк-крушитель")
        {
            Description = "Искажённый тьмой воин, наносящий сокрушительные удары вблизи.",
            damage = 12,
            speed = 5,
            hp = 40,
            color = ColorMobs.Demonic,
            SpeedAttack = 40,
        };


        public static readonly CloseCombatMobsDifinition Orc_Warchief =
        new CloseCombatMobsDifinition("Орк-вожак Зулус")
        {
            Description = "Лидер орков, обладающий тактическим мышлением и огромной силой.",
            damage = 14,
            speed = 5,
            hp = 180,
            ChanceMiss = 15,
            color = ColorMobs.Boss,
            SpeedAttack = 35,
            DistanceAttack = 120,
        };

        //=====================================================

        public static readonly CloseCombatMobsDifinition Orc_Berserker =
        new CloseCombatMobsDifinition("Орк-берсерк")
        {
            Description = "Безумный боец, теряющий контроль в бою и идущий напролом.",
            damage = 7,
            speed = 8,
            hp = 160,
            ChanceMiss = 5,
            color = ColorMobs.Brutal,
            SpeedAttack = 20,
        };

        public static readonly CloseCombatMobsDifinition Orc_Veteran =
        new CloseCombatMobsDifinition("Орк-ветеран")
        {
            Description = "Закалённый войнами орк, умеющий уклоняться и ждать момент для удара.",
            damage = 6,
            speed = 5,
            hp = 120,
            ChanceMiss = 20,
            color = ColorMobs.Elite,
            SpeedAttack = 35,
        };

        

        public static readonly CloseCombatMobsDifinition Orc_Brute =
        new CloseCombatMobsDifinition("Орк-громила")
        {
            Description = "Массивный орк с грубой силой, способный выдержать множество ударов.",
            damage = 10,
            speed = 3,
            hp = 240,
            color = ColorMobs.Elite,
            SpeedAttack = 60,
        };

        public static readonly CloseCombatMobsDifinition Orc_CorruptedChief =
        new CloseCombatMobsDifinition("Порченный вождь орков Дезмонт")
        {
            Description = "Бывший вождь, поглощённый тьмой. Почти не чувствует боли.",
            damage = 20,
            speed = 5,
            hp = 450,
            ChanceMiss = 5,
            color = ColorMobs.Boss,
            SpeedAttack = 30,
            DistanceAttack = 140,
        };

        //==========================================================

        public static readonly CloseCombatMobsDifinition Orc_Twisted =
        new CloseCombatMobsDifinition("Орк-искажённый")
        {
            Description = "Порождение тёмных ритуалов. Его движения непредсказуемы.",
            damage = 9,
            speed = 6,
            hp = 320,
            color = ColorMobs.Demonic,
            SpeedAttack = 30,
        };

        public static readonly CloseCombatMobsDifinition Orc_Potroshitel =
        new CloseCombatMobsDifinition("Орк-потрошитель")
        {
            Description = "Порождение тёмных ритуалов. Его движения непредсказуемы.",
            damage = 15,
            speed = 5,
            hp = 280,
            color = ColorMobs.Demonic,
            SpeedAttack = 30,
        };

        public static readonly CloseCombatMobsDifinition Orc_Dark =
        new CloseCombatMobsDifinition("Темный-орк")
        {
            Description = "Порождение тёмных ритуалов. Его движения непредсказуемы.",
            damage = 9,
            speed = 6,
            hp = 450,
            color = ColorMobs.Demonic,
            SpeedAttack = 30,
        };

        public static readonly CloseCombatMobsDifinition Orc_BloodOverlord =
        new CloseCombatMobsDifinition("Владыка кровавого племени Нобиус")
        {
            Description = "Древний архибосс орков. Его присутствие внушает страх даже своим.",
            damage = 28,
            speed = 6,
            hp = 1200,
            ChanceMiss = 0,
            color = ColorMobs.ArchBoss,
            SpeedAttack = 20,
            DistanceAttack = 200,
        };
    }

    public class CloseCombatMobsDifinition
    {
        public string Name;
        public string Description; 
        public TextureManager texture;
        public Color color = ColorMobs.Common;
        public int damage = 0;

        public double ChanceMiss = 0;

        public bool IsCanMove = true;
        public int speed = 4;
        public int hp = 20;
        public double DistanceAttack = 70;
        public int SpeedAttack = 30;

        public CloseCombatMobsDifinition(string name)
        {
            Name = name;
        }
    }
}