using System;

namespace WonderLab.Extensions.Hosting.UI;

public record PageDescriptor(Type PageType, Type ViewModelType = default);