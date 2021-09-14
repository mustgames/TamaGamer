using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // jag använder ett library för att kunna spara karaktären, förstår inte libraiyt helt men lyckades få det att fungera med en guide.
namespace tamagoatshit
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory; // stället jag sparar spar filen i
            // Använder en gamer running while loop för att kunna starta om splet.
            bool gameRunning = true;
            while (gameRunning)
            {
                Gamer gamer = new Gamer();
                var exists = File.Exists(path + "gamer.save"); // kollar ifall det finns en save file
                if (exists == false)
                {
                    string setName = Tutorial();
                    gamer.name = setName;
                    gamer.dopamine = 80;
                    gamer.food = 30;
                    gamer.sleep = 90;
                    gamer.homework = 10;
                }
                else
                {
                    FileStream fs = new FileStream(path + "gamer.save", FileMode.Open); // öpnnar filen
                    BinaryFormatter formatter = new BinaryFormatter(); // översätter filen
#pragma warning disable
                    gamer = (Gamer)formatter.Deserialize(fs); // laddar in spelarens gammla data
                    fs.Close();

                    Console.WriteLine("welcome back!");
                }


                var commands = new List<Command>(); // Tycker ändo att de blev ganska smidigt att lägga till nya commandon. Eftersom allt man behöver göra för ett nytt kommando är att göra en ny instans av klassen command så blir det bara en rad.
                commands.Add(new Command() { commandName = "Go to Sleep ( 20- in food, 100+ in sleep, 20 + in homework)", changeFood = -20, changeSleep = 100, changeHomework = 20 });
                commands.Add(new Command() { commandName = "Go to School (50- in dopamine, 20+ in food, 20- in sleep, 50- in homework)", changeDopamine = -50, changeFood = 20, changeHomework = -50 });
                commands.Add(new Command() { commandName = "Eat (10+ in dopamine, 100+ in food, 10- in sleep, 10 + in homework)", changeDopamine = 10, changeFood = 100, changeSleep = -10, changeHomework = 10 });
                commands.Add(new Command() { commandName = "Do homework (15- in dopamine, 10- in food, 10- in sleep, 25 - in homework )", changeDopamine = -15, changeFood = -10, changeSleep = -10, changeHomework = -25 });
                commands.Add(new Command() { commandName = "Play Winnie The Pooh Homerun Derby for 5 hours(50+ in dopamine, 30- in food, 30- in sleep, 30 + in homework)", changeDopamine = 50, changeFood = -30, changeSleep = -30, changeHomework = 30 });
                commands.Add(new Command() { commandName = "Binge all night and do all homework (60- in dopamine, 30 - in food, 40- in sleep 100 - in homework)", changeDopamine = -60, changeFood = -30, changeSleep = -40, changeHomework = -100 });
                commands.Add(new Command() { commandName = "Binge all night and play dinopace at Connextgames.com (100+ in dopamine, 30- in food, 40- in sleep 40 + in homework)", changeDopamine = 100, changeFood = -30, changeSleep = -40, changeHomework = 40 });

                while (gamer.doseNotHaveF) // Min riktiga game running typ void run i unity
                {
                    gamer.ShowStats();
                    Console.WriteLine("Today " + gamer.name.ToString() + " Will do");
                    for (int i = 0; i < commands.Count; i++)
                    {
                        Console.WriteLine((i + 1).ToString() + ". " + commands[i].commandName); // skriver ut alla listans klass instancers namn
                    }


                    int index = 0;
                    bool inputRecived = false;
                    while (!inputRecived) // låter spelaren välja vilken action de vill ta utan att de krashar
                    {
                        Console.WriteLine("Choose something by entering the number before the action you want");
                        var answer = Console.ReadLine();
                        Int32.TryParse(answer, out index);
                        if (index <= commands.Count && index > 0)
                        {
                            inputRecived = true;
                        }
                    }
                    var result = commands[index - 1].doSomething(gamer.dopamine, gamer.food, gamer.sleep, gamer.homework);
                    gamer.dopamine = result.Item1;
                    gamer.food = result.Item2;
                    gamer.sleep = result.Item3;
                    gamer.homework = result.Item4;


                    Console.Clear();
                    gamer.CheckDopamine();
                    gamer.CheckFood();
                    gamer.CheckSleep();
                    gamer.CheckF();

                    // sparar alla förändringar i spar filen
                    var fs = new FileStream(path + "gamer.save", FileMode.Create);
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(fs, gamer);
                    fs.Close();

                }
                Console.WriteLine("Your gamer " + gamer.name + " got a F, you lose");
                Console.WriteLine("Type exit to exit or anything else to play again");
                File.Delete(path + "gamer.save");// tar bort filen så spelaren inte kan stänga av programmet ifall de dog och gå tillbaka till sin gammla spar fil.
                string exitString = Console.ReadLine();
                if (exitString == "exit")
                {
                    gameRunning = false;
                }
            }
            // Jag ville bryta ut tutorial som en egen metod och eftersom det ända som sätts i tutrial är namnet så returnerar jag en string.
            static string Tutorial()
            {
                Console.WriteLine("Welcome to the tutorial of Tamagamer");
                Console.WriteLine("Name your gamer!");
                string name = Console.ReadLine();
                Console.WriteLine("In this game you take care of your gamer " + name);
                Console.WriteLine("If your gamers homework reaches 100, they get a F and you lose.");
                Console.WriteLine("You decrease homework by forcing your gamer to do homework");
                Console.WriteLine("You will also need to keep track of your gamers Dopamine, Food and Sleep");
                Console.WriteLine("If your gamer can last a year witout a F you win");
                Console.WriteLine("GL HF");
                Console.WriteLine("Press anything to continue");
                Console.ReadKey();
                Console.Clear();
                return name;
            }
        }
    }

    [Serializable] // Gör så att klassen och dess värden går att spara.
    public class Gamer
    {
        public bool doseNotHaveF = true;
        public string name;
        public int dopamine;
        public int food;
        public int sleep;
        public int homework;

        public void ShowStats() // klassens metod för att skriva ut sina värden
        {
            Console.WriteLine("Stats:");
            Console.WriteLine();
            Console.WriteLine("Dopamine: " + this.dopamine.ToString() + "/100");
            Console.WriteLine("Food: " + this.food.ToString() + "/100");
            Console.WriteLine("Sleep: " + this.sleep + "/100");
            Console.WriteLine("Homework: " + this.homework + "/100");
        }
        public void CheckF() // kollar ifall spelaren har förlorat 
        {
            if (this.homework > 99)
            {
                this.doseNotHaveF = false;
            }
            if (this.homework < 0)
            {
                this.homework = 0;
            }

            // Behöver kolla ifall all stats är mer än max eller mindre än minst igen eftersom annars kan bonnusarna från de andra checksen få statsen att bli för mycekt eller för lite.
            // Ser dumt ut men det är det sisata jag gör och de funkar.
            if (this.dopamine > 100)
            {
                this.dopamine = 100;
            }
            if (this.homework < 0)
            {
                this.homework = 0;
            }

            if (this.food > 100)
            {
                this.food = 100;
            }
            if (this.food < 0)
            {
                this.food = 0;
            }

            if (this.sleep > 100)
            {
                this.sleep = 100;
            }
            if (this.sleep < 0)
            {
                this.sleep = 0;
            }
        }
        // Iden med checkens är ifall spelaren ignorerar någon stat kommer de bli straffade.
        public void CheckDopamine()
        {
            if (this.dopamine > 100)
            {
                this.dopamine = 100;
            }
            if (this.dopamine < 0)
            {
                Console.WriteLine("Your gamer got to board and satrted browsing reddit(40+ in dopamine) 25- in the other stats, 30+ in homework");
                this.dopamine = 40;
                this.food = this.food - 25;
                this.sleep = this.sleep - 25;
                this.homework = this.homework + 30;
                ShowStats();
            }
        }
        public void CheckFood()
        {
            if (this.food > 100)
            {
                this.food = 100;
            }
            if (this.food < 0)
            {
                Console.WriteLine("Your gamer got to hungry and had a snicker (40+ in food) 25- in the other stats, 30+ in homework");
                this.dopamine = this.dopamine - 25;
                this.food = 40;
                this.sleep = this.sleep - 25;
                this.homework = this.homework + 30;
                ShowStats();
            }
        }
        public void CheckSleep()
        {
            if (this.sleep > 100)
            {
                this.sleep = 100;
            }
            if (this.sleep < 0)
            {
                Console.WriteLine("Your gamer got to sleepy and took a nap (40+ in food) 25- in the other stats, 30+ in homework");
                this.dopamine = this.dopamine - 25;
                this.food = this.food - 25;
                this.sleep = 40;
                this.homework = this.homework + 30;
                ShowStats();
            }
        }
    }
    // Märkte att det egentligen inte fanns någon annledning att lägga commandona i en class, gjorde det bara för jag ville testa att göra flera klasser.
    class Command
    {
        public string commandName;
        public int changeDopamine;
        public int changeFood;
        public int changeSleep;
        public int changeHomework;

        public (int, int, int, int) doSomething(int amount1, int amount2, int amount3, int amount4)
        {
            Console.WriteLine(this.commandName);
            int newAmount1 = amount1 + changeDopamine;
            int newAmount2 = amount2 + changeFood;
            int newAmount3 = amount3 + changeSleep;
            int newAmount4 = amount4 + changeHomework;

            return (newAmount1, newAmount2, newAmount3, newAmount4);
        }
    }
}
