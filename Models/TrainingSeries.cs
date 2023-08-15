﻿namespace gFit.Models
{
	public class TrainingSeries
	{
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public int PersonalId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Series { get; set; }
        public int Repetitions { get; set; }
        public float? RestTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User? User { get; set; }
        public Personal? Personal { get; set; }
        public List<Exercise>? Exercises { get; set; }
    }
}
