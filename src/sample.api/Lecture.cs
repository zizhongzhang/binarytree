﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sample.api
{
    public class Lecture
    {
        [Required]
        public string Name { get; set; }
        public DateTime? LectureDate { get; set; }
    }

    public class LectureValidator : AbstractValidator<Lecture>
    {
        public LectureValidator()
        {
            RuleFor(x => x.LectureDate).NotNull();
            RuleFor(x => x.LectureDate).GreaterThan(DateTime.MinValue);
        }
    }

    public class Person
    {
        public IList<Pet> Pets { get; set; } = new List<Pet>();
    }
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Pets).Must(list => list.Count <= 10)
              .WithMessage("The list must contain fewer than 10 items");
        }
    }
}
