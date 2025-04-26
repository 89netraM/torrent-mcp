using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace TorrentMcp.FloodIntegration;

internal class FloodClientOptions
{
    [Required]
    public required Uri BaseUrl { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }
}

[OptionsValidator]
internal partial class ValidateFloodClientOptions : IValidateOptions<FloodClientOptions>;
