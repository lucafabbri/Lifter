namespace Lifter.Avalonia;

/// <summary>
/// Marker interface for views that can be hosted in a <see cref="HostedApplication{TMainView}"/>.
/// </summary>
/// <remarks>
/// This interface is used as a compile-time constraint to ensure that only appropriate
/// views are used with the HostedApplication framework.
/// </remarks>
public interface IHostedView
{
}
