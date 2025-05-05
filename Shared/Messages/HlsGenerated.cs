using System;

namespace Shared.Messages;

public record HlsGenerated(Guid VideoId, string HlsPath);