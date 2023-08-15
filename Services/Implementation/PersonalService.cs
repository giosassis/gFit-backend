﻿using System.Text;
using AutoMapper;
using gFit.Models;
using gFit.Repositories.Interface;
using gFit.Services.Interfaces;
using System.Security.Cryptography;
using gFit.Validator;
using static gFit.Context.DTOs.PersonalDto;
using gFit.Services.Interface;
using static gFit.Context.DTOs.UserDto;

namespace gFit.Services.Implementation
{
    public class PersonalService : IPersonalService
    {
        private readonly IMapper _mapper;
        private readonly IPersonalRepository _personalRepository;

        public PersonalService(IMapper mapper, IPersonalRepository personalRepository)
        {
            _mapper = mapper;
            _personalRepository = personalRepository;
        }

        public async Task<IEnumerable<PersonalReadDTO>> GetAllPersonalsAsync()
        {
            var personals = await _personalRepository.GetAllPersonalsAsync();
            return _mapper.Map<IEnumerable<PersonalReadDTO>>(personals);
        }

        public async Task<PersonalReadDTO> GetPersonalByIdAsync(Guid id)
        {
            var personal = await _personalRepository.GetPersonalByIdAsync(id);
            return _mapper.Map<PersonalReadDTO>(personal);
        }

        public async Task<PersonalReadDTO> CreatePersonalAsync(PersonalCreateDTO personalCreateDTO)
        {
            if (!IsValidEmail(personalCreateDTO.Email))
            {
                throw new ArgumentException("Invalid email format");
            }

            if (!PasswordValidator.Validate(personalCreateDTO.Password))
            {
                throw new ArgumentException("Password must meet security requirements.");
            }

            string hashedPassword = PasswordHasher(personalCreateDTO.Password);

            var personal = _mapper.Map<Personal>(personalCreateDTO);
            personal.Password = hashedPassword;

            var createdPersonal = await _personalRepository.CreatePersonalAsync(personal);
            return _mapper.Map<PersonalReadDTO>(createdPersonal);
        }

        public async Task<PersonalReadDTO> UpdatePersonalAsync(Guid id, PersonalUpdateDTO personalUpdateDTO)
        {
            var existingPersonal = await _personalRepository.GetPersonalByIdAsync(id);

            if (existingPersonal == null)
            {
                throw new Exception("Personal not found");
            }

            var updatedPersonal = await _personalRepository.UpdatePersonalAsync(id, existingPersonal);
            return _mapper.Map<PersonalReadDTO>(updatedPersonal);
        }

        public async Task DeletePersonalAsync(Guid id)
        {
            var existingPersonal = await _personalRepository.GetPersonalByIdAsync(id);

            if (existingPersonal == null)
            {
                return;
            }

            await _personalRepository.DeletePersonalAsync(id);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static string PasswordHasher(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}