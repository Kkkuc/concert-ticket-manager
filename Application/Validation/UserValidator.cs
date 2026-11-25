using Application.DTOs;
using System.Globalization;

namespace Application.Validation;

public class UserValidator
{
    public Dictionary<string, string> Validate(RegisterRequest user)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(user.FirstName))
            errors["first_name"] = "Imię jest wymagane.";

        if (string.IsNullOrWhiteSpace(user.LastName))
            errors["last_name"] = "Nazwisko jest wymagane.";

        if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains('@'))
            errors["email"] = "Niepoprawny e-mail.";

        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            if (user.PhoneNumber.Length != 9 ||
                !long.TryParse(user.PhoneNumber, NumberStyles.None,
                    CultureInfo.InvariantCulture, out _))
                errors["phone_number"] = "Telefon musi mieć 9 cyfr.";
        }

        if (user.Password == null || user.Password.Length < 8)
            errors["password"] = "Hasło musi mieć minimum 8 znaków.";

        if (user.Password != user.ConfirmPassword)
            errors["confirm_password"] = "Hasła nie są identyczne.";

        return errors;
    }
}
