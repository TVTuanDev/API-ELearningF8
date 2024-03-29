﻿using System.Text;

namespace ELearningF8.Models
{
    public class RandomGenerator
    {
        private static readonly Random random = new Random();
        private const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string RandomCode()
        {
            StringBuilder code = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                code.Append(characters[random.Next(characters.Length)]);
            }
            return code.ToString();
        }
    }
}
