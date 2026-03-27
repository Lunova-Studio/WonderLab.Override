using System;

namespace WonderLab.Services.Navigation;

public record PageDescriptor(Type PageType, Type ViewModelType = default);