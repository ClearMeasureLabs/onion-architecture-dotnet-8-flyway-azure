using Palermo.BlazorMvc;

namespace UI.Shared.Models;

public record UserLoggedInEvent(string LoginModelUsername) : IUiBusEvent;