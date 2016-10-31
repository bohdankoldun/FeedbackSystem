using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FeedbackSystem.Models
{
    public class Idea
    {
        public string Id { get; set; }

        [Required]
        public string DescriptionIdea { get; set; }

        public DateTime Date { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public ICollection<Vote> Votes { get; set; }
        public Idea() 
        {
            Votes = new List<Vote>();
        }


    }

    public class Vote
    {
        public string Id { get; set; }

        [Required]
        public bool  _Vote { get; set; }

        public string IdeaId { get; set; }
        [ForeignKey("IdeaId")]
        public Idea Idea { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }


    }

    public class IdeaToView
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string DescriptionIdea { get; set; }

        public string Date { get; set; }

        public int PositiveVotes { get; set; }

        public int NegativeVotes { get; set; }
       


    }

}