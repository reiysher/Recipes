﻿using StateSync.Api.Shared.Abstractions;

namespace StateSync.Api.Shared.Persistence;

public interface ISyncProjectionHandler
{
    bool CanHandle(IProjectionSource entity);

    Task Handle(IProjectionSource entity);
}
