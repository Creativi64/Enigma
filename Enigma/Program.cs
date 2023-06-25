using System.Data;
using System.Diagnostics;
using System.Text;

namespace Enigma
{
	public class Program
	{
		private static EnigmaSetup enigmaSetup = new()
		{
			Permutations = new List<CharacterPair>()
			{
				new CharacterPair('a', 't'),
				new CharacterPair('b', 'l'),
				new CharacterPair('d', 'f'),
				new CharacterPair('g', 'j'),
				new CharacterPair('h', 'm'),
				new CharacterPair('n', 'w'),
				new CharacterPair('o', 'p'),
				new CharacterPair('q', 'y'),
				new CharacterPair('r', 'z'),
				new CharacterPair('v', 'x'),
			},
			ReverserRoller = new RollerSetup()
			{
				Name = "Reverser",
				RollerCharacters = EnigmaSetup.Umkehrwalze_B,
			},
			RollerSetups = new List<RollerSetup>()
			{
				// Oben nach unten ist rechts nach links
				
				new RollerSetup() {
					Name = "ETW",
					RollerCharacters =EnigmaSetup.defaultRoller,
					ShiftCharacters = EnigmaSetup.defaultRoller,
				},
				new RollerSetup()
				{
					Name = "FIRST",
					ShiftCharShift='v',
					InitialShift='a',
					ShiftCharacters = new List<char>(){'y'},
					RollerCharacters = EnigmaSetup.I
				},
				new RollerSetup()
				{
					Name = "Second",
					ShiftCharShift='a',
					InitialShift = 'n',
					ShiftCharacters = new List<char>(){'r'},
					RollerCharacters = EnigmaSetup.IV
				},
				new RollerSetup()
				{
					Name = "Third",
					ShiftCharShift='a',
					InitialShift = 'j',
					ShiftCharacters = new List<char>(){'m'},
					RollerCharacters = EnigmaSetup.II
				},
				new RollerSetup()
				{
					Name = "BETAROller",
					ShiftCharShift='a',
					InitialShift = 'v',
					ShiftCharacters = new List<char>(),
					RollerCharacters = EnigmaSetup.Beta
				},

			}
		};

		static void Main()
		{
			Console.OutputEncoding = Encoding.UTF8;
			Console.WriteLine("start");
			var enigma = new Enigma(enigmaSetup);
			var enigma2 = new Enigma(enigmaSetup);
			//var enc = "ttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt";
			//var h = File.ReadAllText(@".\lorem.txt").ToLower();
			var enc = "NCZW".Replace(" ", "").ToLower();
			Stopwatch t = new();
			t.Start();
			string encrypted = enigma.Crypt(enc);
			//enigma2.printCurrentConfig();
			string _encrypted = enigma2.Crypt(encrypted);

			t.Stop();
			//Console.WriteLine(h.Length);
			Console.WriteLine(t.ElapsedMilliseconds);
			Console.WriteLine(encrypted);
			Console.WriteLine("----");
			Console.WriteLine(_encrypted);
			Console.ReadLine();
		}
	}

	public class Enigma
	{
		private readonly List<Roller> configuration;
		private readonly List<CharacterPair> permutations;

		public Enigma(EnigmaSetup setup)
		{
			permutations = setup.Permutations;
			configuration = setup.GetRollerSetup();
		}

		public string Crypt(string input)
		{
			var output = new StringBuilder();
			foreach (char c in input)
			{
				bool shiftNextCharacter = false;

				Queue<char?> chosenCharacters = new();
				chosenCharacters.Enqueue(c);

				char tempChar = permutate(c);
				int charPos = EnigmaSetup.defaultRoller.IndexOf(tempChar);

				// Forwards
				foreach (var walze in configuration)
				{
					chosenCharacters.Enqueue(tempChar);
					(charPos, tempChar, shiftNextCharacter) = walze.CryptCharacter(charPos, shiftNextCharacter);
				}

				// Backwards
				for (int i = configuration.Count - 2; i >= 0; i--)
				{
					chosenCharacters.Enqueue(tempChar);
					(charPos, tempChar, _) = configuration[i].CryptCharacter(charPos, false, true);
				}

				chosenCharacters.Enqueue(tempChar);
				tempChar = permutate(tempChar);

				output.Append(tempChar);
			}
			return output.ToString();
		}

		private void printCurrentConfig(Queue<char?>? ch = null)
		{
			var list = new List<(List<CharacterPair>, string)>();
			foreach (var item in configuration)
			{
				list.Add(item.GetState());
			}

			Console.WriteLine($"{"",-10}:0-1-2-3-4-5-6-7-8-9-10111213141516171819202122232425");
			//Console.WriteLine($"{"",-10}:1-2-3-4-5-6-7-8-9-1011121314151617181920212223242526");
			Console.WriteLine($"{"Permu",-10}:{string.Join('-', EnigmaSetup.defaultRoller.ToList())}");
			Console.WriteLine($"{"▼".PadLeft(12 + EnigmaSetup.defaultRoller.IndexOf((char)(ch?.Dequeue() ?? 'a')) * 2)}");

			var li = new List<char>();
			foreach (var item in EnigmaSetup.defaultRoller)
			{
				var t = permutations.Find(n => n.First == item || n.Second == item);
				if (t == null)
				{
					li.Add(item);
					continue;
				}
				if (t.First == item)
				{
					li.Add(t.Second);
					continue;
				}
				li.Add(t.First);
			}
			Console.WriteLine($"{"",-10}:{string.Join('-', li)}");

			for (int i = 0; i < list.Count; i++)
			{
				Console.WriteLine("---------------------------------------------------------");
				Console.WriteLine($"{"▼".PadLeft(12 + EnigmaSetup.defaultRoller.IndexOf((char)(ch?.Dequeue() ?? 'a')) * 2)}");

				Console.WriteLine($"{list[i].Item2,-10}:{string.Join('-', list[i].Item1.Select(n => n.First))}");
				Console.WriteLine($"{"",-10}:{string.Join('-', list[i].Item1.Select(n => n.Second))}");
			}
		}

		private char permutate(char input)
		{
			var permutation = permutations.Find(n => n.First == input || n.Second == input);
			if (permutation == null)
				return input;

			if (input == permutation.First)
				return permutation.Second;
			return permutation.First;
		}
	}

	public class EnigmaSetup
	{
		// https://www.cryptomuseum.com/crypto/enigma/wiring.htm
		public readonly static List<char> defaultRoller = new() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

		public readonly static List<char> Umkehrwalze_D = new() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
		public readonly static List<char> Umkehrwalze_B = new() { 'e', 'n', 'k', 'q', 'a', 'u', 'y', 'w', 'j', 'i', 'c', 'o', 'p', 'b', 'l', 'm', 'd', 'x', 'z', 'v', 'f', 't', 'h', 'r', 'g', 's' };
		public readonly static List<char> Umkehrwalze_C = new() { 'a', 'd', 'o', 'b', 'j', 'n', 't', 'k', 'v', 'e', 'h', 'm', 'l', 'f', 'c', 'w', 'z', 'a', 'x', 'g', 'y', 'i', 'p', 's', 'u', 'q' };

		//public static List<char> =          new() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
		public readonly static List<char> I = new() { 'e', 'k', 'm', 'f', 'l', 'g', 'd', 'q', 'v', 'z', 'n', 't', 'o', 'w', 'y', 'h', 'x', 'u', 's', 'p', 'a', 'i', 'b', 'r', 'c', 'j' };
		public readonly static List<char> II = new() { 'a', 'j', 'd', 'k', 's', 'i', 'r', 'u', 'x', 'b', 'l', 'h', 'w', 't', 'm', 'c', 'q', 'g', 'z', 'n', 'p', 'y', 'f', 'v', 'o', 'e' };
		public readonly static List<char> III = new() { 'b', 'd', 'f', 'h', 'j', 'l', 'c', 'p', 'r', 't', 'x', 'v', 'z', 'n', 'y', 'e', 'i', 'w', 'g', 'a', 'k', 'm', 'u', 's', 'q', 'o' };
		public readonly static List<char> IV = new() { 'e', 's', 'o', 'v', 'p', 'z', 'j', 'a', 'y', 'q', 'u', 'i', 'r', 'h', 'x', 'l', 'n', 'f', 't', 'g', 'k', 'd', 'c', 'm', 'w', 'b' };
		public readonly static List<char> V = new() { 'v', 'z', 'b', 'r', 'g', 'i', 't', 'y', 'u', 'p', 's', 'd', 'n', 'h', 'l', 'x', 'a', 'w', 'm', 'j', 'q', 'o', 'f', 'e', 'c', 'k' };

		public readonly static List<char> Beta = new() { 'l', 'e', 'y', 'j', 'v', 'c', 'n', 'i', 'x', 'w', 'p', 'b', 'q', 'm', 'd', 'r', 't', 'a', 'k', 'z', 'g', 'f', 'u', 'h', 'o', 's' };
		public readonly static List<char> Gamma = new() { 'f', 's', 'o', 'k', 'a', 'n', 'u', 'e', 'r', 'h', 'm', 'b', 't', 'i', 'y', 'c', 'w', 'l', 'q', 'p', 'z', 'x', 'v', 'g', 'j', 'd' };

		public List<RollerSetup> RollerSetups { get; set; } = new List<RollerSetup>();
		public RollerSetup ReverserRoller { get; set; } = new RollerSetup();
		public List<CharacterPair> Permutations { get; set; } = new List<CharacterPair>();

		public List<Roller> GetRollerSetup()
		{
			var tempSetup = new List<Roller>();

			foreach (var _setup in RollerSetups)
			{
				tempSetup.Add(new Roller(defaultRoller, _setup.RollerCharacters, _setup.InitialShift, _setup.ShiftCharShift, _setup.ShiftCharacters, _setup.Name));
			}
			tempSetup.Add(new Roller(defaultRoller, ReverserRoller.RollerCharacters, ReverserRoller.InitialShift, ReverserRoller.ShiftCharShift, ReverserRoller.ShiftCharacters, ReverserRoller.Name));
			return tempSetup;
		}
	}

	public class Roller
	{
		public string Name { get; set; }
		public int InitialShift { get; }
		public int InitialShiftShift { get; }

		private List<CharacterPair> characterList = new();
		private List<char> ShiftCharacters { get; set; } = new List<char>();

		private int effectiveRollerShift = 0;

		public int EffectiveRollerShift
		{
			get { return effectiveRollerShift; }
		}

		public Roller(List<char> defaultSet, List<char> mutationSet, char? initialShift, char? initialShiftShift, List<char> shiftCharacters, string name)
		{
			// default -> a
			this.InitialShiftShift = 0;

			foreach (var item in shiftCharacters)
			{
				int pos = EnigmaSetup.defaultRoller.IndexOf(item);
				if (initialShiftShift.HasValue)
				{
					int shift = EnigmaSetup.defaultRoller.IndexOf(initialShiftShift.Value);
					this.InitialShiftShift = shift;
					pos += shift;
					if (pos > EnigmaSetup.defaultRoller.Count)
					{
						pos -= EnigmaSetup.defaultRoller.Count;
					}
				}

				ShiftCharacters.Add(EnigmaSetup.defaultRoller.ElementAt(pos));
			}

			for (int i = 0; i < defaultSet.Count; i++)
			{
				characterList.Add(new CharacterPair(defaultSet[i], mutationSet[i]));
			}

			int shifting = 0;
			if (initialShift.HasValue)
				shifting = EnigmaSetup.defaultRoller.IndexOf(initialShift.Value);
			int a = 0;
			for (int i = 0; i < shifting; i++)
			{
				a++;
				shift();
			}
			this.Name = name;
			this.InitialShift = shifting;
		}

		/// <summary>
		/// Crypts a character
		/// </summary>
		/// <param name="character"></param>
		/// <param name="shiftAllowed"></param>
		/// <param name="reverse"></param>
		/// <returns>index of the next char, the current char, if the next roller is able to turn</returns>
		/// <exception cref="Exception"></exception>
		public (int, char, bool) CryptCharacter(int character, bool shiftAllowed, bool reverse = false)
		{
			if (!reverse)
			{
				// Shift next if this top char is shift char
				if (shiftAllowed)
					shift();

				char inChar = characterList.ElementAt(character).First;
				bool turnNext = ShiftCharacters.Contains(this.characterList[characterList.Count - 1].First);

				// Only when turning
				if (InitialShiftShift != 0)
				{
					// Determine input Char, SHIFT IN
					character -= this.InitialShiftShift;
					if (character < 0)
						character += EnigmaSetup.defaultRoller.Count;
				}

				// call Permutation
				var pair = characterList.ElementAt(character);
				if (pair != null)
				{
					int nextChar = characterList.FindIndex(n => n.First == pair.Second);

					// Only when turning
					if (InitialShiftShift != 0)
					{
						// Determine output Char, SHIFT OUT
						nextChar += this.InitialShiftShift;
						if (nextChar > EnigmaSetup.defaultRoller.Count - 1)
							nextChar -= EnigmaSetup.defaultRoller.Count;
					}
					return (nextChar, inChar, turnNext);
				}
			}
			else
			{
				char inChar = characterList.ElementAt(character).First;

				// Only when turning
				if (InitialShiftShift != 0)
				{
					// Determine input Char, SHIFT IN
					character -= this.InitialShiftShift;
					if (character < 0)
						character += EnigmaSetup.defaultRoller.Count;
				}

				var pair = characterList.ElementAt(character);
				if (pair != null)
				{
					int nextChar = characterList.FindIndex(n => n.Second == pair.First);

					// Only when turning
					if (InitialShiftShift != 0)
					{
						// Determine output Char, SHIFT OUT
						nextChar += this.InitialShiftShift;
						if (nextChar > EnigmaSetup.defaultRoller.Count - 1)
							nextChar -= EnigmaSetup.defaultRoller.Count;
					}
					return (nextChar, inChar, false);
				}
			}
			throw new Exception("sth should be here");
		}

		public (List<CharacterPair>, string) GetState()
		{
			return (characterList, this.Name);
		}

		private void shift(bool log = false)
		{
			if (log)
				Console.Write($" <{Name} Shifted>");

			var t = characterList.First();
			if (t != null)
			{
				characterList.Remove(t);
				characterList = characterList.Append(t).ToList();
				effectiveRollerShift++;
				return;
			}
			throw new Exception("Illegal move");
		}

	}

	public class RollerSetup
	{
		public char? InitialShift { get; set; } = 'a';
		public char? ShiftCharShift { get; set; } = 'a';

		public List<char> ShiftCharacters { get; set; } = new();
		public string Name { get; set; } = "";
		public List<char> RollerCharacters { get; set; } = new List<char>();
	}

	public class CharacterPair
	{
		public CharacterPair(char firstCharacter, char secondCharacter)
		{
			First = firstCharacter;
			Second = secondCharacter;
		}

		public char First { get; set; }
		public char Second { get; set; }

		public override string ToString()
		{
			return $"{First}><{Second}";
		}
	}
}
