using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WonderLab.Classes.Enums;
using WonderLab.Extensions;

namespace WonderLab.Controls.Experimental.Effect;

public class SkiaEffect {
    // Basic uniforms passed into the shader from the CPU.
    private static readonly string[] Uniforms = [
        "uniform float iTime;",
        "uniform float iDark;",
        "uniform float iAlpha;",
        "uniform vec3 iResolution;",
        "uniform vec3 iPrimary;",
        "uniform vec3 iAccent;",
        "uniform vec3 iBase;"
    ];

    private static bool _disposed;
    private static BackgroundType _type;
    private static readonly float[] White = [0.95f, 0.95f, 0.95f];
    private static readonly List<SkiaEffect> LoadedEffects = [];

    private readonly string _shaderString;
    private readonly string _rawShaderString;
    private readonly float[] _boundsAlloc = new float[3];
    private readonly float[] _backgroundAlloc = new float[3];
    private readonly float[] _backgroundAccentAlloc = new float[3];
    private readonly float[] _backgroundPrimaryAlloc = new float[3];

    /// <summary>
    /// The compiled <see cref="SKRuntimeEffect"/> that will actually be used in draw calls. 
    /// </summary>
    public SKRuntimeEffect Effect { get; }
    //error: 85: unknown identifier 'smoothstep'\n1 error\n
    private SkiaEffect(string shaderString, string rawShaderString) {
        _shaderString = shaderString;
        _rawShaderString = rawShaderString;
        var compiledEffect = SKRuntimeEffect.Create(_shaderString, out var errors);
        Effect = compiledEffect ?? throw new ShaderCompilationException(errors);
    }

    static SkiaEffect() {
        if (Application.Current.ApplicationLifetime is IControlledApplicationLifetime controlled)
            controlled.Exit += (_, _) => EnsureDisposed();
    }

    /// <summary>
    /// Attempts to load and compile a ".sksl" shader file from the assembly.
    /// You don't need to provide the extension.
    /// The shader will be pre-compiled
    /// REMEMBER: For files to be discoverable in the assembly they should be marked as an embedded resource.
    /// </summary>
    /// <param name="shaderName">Name of the shader to load, with or without extension. - MUST BE .sksl</param>
    public static SkiaEffect FromEmbeddedResource(string shaderName, BackgroundType type = BackgroundType.Voronoi) {
        _type = type;
        shaderName = shaderName.ToLowerInvariant();
        if (!shaderName.EndsWith(".sksl"))
            shaderName += ".sksl";

        var assembly = Assembly.GetEntryAssembly();
        var resName = assembly?.GetManifestResourceNames()
            .FirstOrDefault(x => x.Contains(shaderName, StringComparison.InvariantCultureIgnoreCase));

        if (resName is null) {
            assembly = Assembly.GetExecutingAssembly();
            resName = assembly?.GetManifestResourceNames()
                .FirstOrDefault(x => x.Contains(shaderName, StringComparison.InvariantCultureIgnoreCase));
        }

        if (resName is null) {
            assembly = typeof(SkiaEffect).Assembly;
            resName = assembly?.GetManifestResourceNames()
                .FirstOrDefault(x => x.Contains(shaderName, StringComparison.InvariantCultureIgnoreCase));
        }

        if (resName is null)
            throw new FileNotFoundException(
                $"Unable to find a file with the name \"{shaderName}\" anywhere in the assembly.");

        using var tr = new StreamReader(assembly.GetManifestResourceStream(resName)!);
        return FromString(tr.ReadToEnd());
    }

    /// <summary>
    /// Attempts to compile an sksl shader from a string.
    /// The shader will be pre-compiled and any errors will be thrown as an exception.
    /// REMEMBER: For files to be discoverable in the assembly they should be marked as an embedded resource.
    /// </summary>
    /// <param name="shaderString">The shader code to be compiled.</param>
    /// <returns>An instance of a SukiBackgroundShader with the loaded shader</returns>
    public static SkiaEffect FromString(string shaderString) {
        var sb = new StringBuilder();
        foreach (var uniform in Uniforms)
            sb.AppendLine(uniform);

        sb.Append(shaderString);
        var withUniforms = sb.ToString();
        return new(withUniforms, shaderString);
    }


    /// <summary>
    /// Necessary to make sure all the unmanaged effects are disposed.
    /// </summary>
    internal static void EnsureDisposed() {
        if (_disposed)
            throw new InvalidOperationException(
                "Effects should only be disposed once at the app lifecycle end.");

        _disposed = true;
        foreach (var loaded in LoadedEffects)
            loaded.Effect.Dispose();

        LoadedEffects.Clear();
    }

    public override bool Equals(object obj) {
        if (obj is not SkiaEffect effect) return false;
        return effect._shaderString == _shaderString;
    }

    internal SKShader ToShaderWithUniforms(float timeSeconds, ThemeVariant activeVariant, Rect bounds,
        float animationScale, float alpha = 1f) {

        //Update allocated color arrays.
        if (_type is BackgroundType.Voronoi) {
            Voronoi();
        } else
            Bubble();

        _boundsAlloc[0] = (float)bounds.Width;
        _boundsAlloc[1] = (float)bounds.Height;

        var inputs = new SKRuntimeEffectUniforms(Effect) {
            { "iResolution", _boundsAlloc },
            { "iTime", timeSeconds * animationScale },
            { "iBase", _backgroundAlloc },
            { "iAccent", _backgroundAccentAlloc },
            { "iPrimary", _backgroundPrimaryAlloc },
            { "iDark", activeVariant == ThemeVariant.Dark ? 1f : 0f },
            { "iAlpha", alpha }
        };

        return Effect.ToShader(true, inputs);

        void Voronoi() {
            if (activeVariant == ThemeVariant.Dark) {
                App.Monet.DesignTokens.OnSecondaryColorValue.FromUInt32().ToFloatArrayNonAlloc(_backgroundAlloc);
                App.Monet.DesignTokens.BackgroundColorValue.FromUInt32().ToFloatArrayNonAlloc(_backgroundAccentAlloc);
                App.Monet.DesignTokens.PrimaryColorValue.FromUInt32().ToFloatArrayNonAlloc(_backgroundPrimaryAlloc);
            } else {
                App.Monet.DesignTokens.InversePrimaryColorValue.FromUInt32().ToFloatArrayNonAlloc(_backgroundAlloc);
                App.Monet.DesignTokens.OnPrimaryColorValue.FromUInt32().ToFloatArrayNonAlloc(_backgroundPrimaryAlloc);
                App.Monet.DesignTokens.OnPrimaryContainerColorValue.FromUInt32().ToFloatArrayNonAlloc(_backgroundAccentAlloc);
            }
        }

        void Bubble() {
            if (activeVariant == ThemeVariant.Dark) {
                App.Monet.DesignTokens.OnSecondaryColorValue.FromUInt32().ToFloatArrayNonAlloc(_backgroundAlloc);
                App.Monet.DesignTokens.BackgroundColorValue.FromUInt32().ToFloatArrayNonAlloc(_backgroundAccentAlloc);
                App.Monet.DesignTokens.PrimaryColorValue.FromUInt32().ToFloatArrayNonAlloc(_backgroundPrimaryAlloc);
            } else {
                App.Monet.DesignTokens.SecondaryContainerColorValue.FromUInt32().ToFloatArrayNonAlloc(_backgroundAlloc);
                App.Monet.DesignTokens.OnBackgroundColorValue.FromUInt32().ToFloatArrayNonAlloc(_backgroundPrimaryAlloc);
                App.Monet.DesignTokens.TertiaryContainerColorValue.FromUInt32().ToFloatArrayNonAlloc(_backgroundAccentAlloc);
            }
        }
    }

    internal SKShader ToShaderWithCustomUniforms(Func<SKRuntimeEffect, SKRuntimeEffectUniforms> uniformFactory, float timeSeconds, Rect bounds,
        float animationScale, float alpha = 1f) {
        var uniforms = uniformFactory(Effect);
        uniforms.Add("iResolution", new[] { (float)bounds.Width, (float)bounds.Height, 0f });
        uniforms.Add("iTime", timeSeconds * animationScale);
        uniforms.Add("iAlpha", alpha);
        return Effect.ToShader(true, uniforms);
    }

    /// <summary>
    /// Returns the pure shader string without uniforms.
    /// </summary>
    public override string ToString() {
        return _rawShaderString;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    private class ShaderCompilationException(string message) : Exception(message);
}