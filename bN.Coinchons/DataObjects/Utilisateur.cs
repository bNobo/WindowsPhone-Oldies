using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bN.Coinchons.DataObjects
{
	public class Utilisateur
	{
		public string Id { get; set; }
		public string LiveId { get; set; }
		public ICollection<Joueur> Joueurs { get; set; }

		public Utilisateur()
		{
			Joueurs = new HashSet<Joueur>();
		}
	}
}
