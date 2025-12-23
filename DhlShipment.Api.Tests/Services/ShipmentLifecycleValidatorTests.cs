using DhlShipment.Api.Exceptions;
using DhlShipment.Api.Models;
using DhlShipment.Api.Services;
using FluentAssertions;
using Xunit;

namespace DhlShipment.Api.Tests.Services;

public class ShipmentLifecycleValidatorTests
{
    private readonly IShipmentLifecycleValidator _validator;

    public ShipmentLifecycleValidatorTests()
    {
        _validator = new ShipmentLifecycleValidator();
    }

    #region Valid Forward Transitions

    [Fact]
    public void ValidateTransition_Created_To_PickedUp_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.Created,
            ShipmentStatus.PickedUp))
            .Should().NotThrow();
    }

    [Fact]
    public void ValidateTransition_PickedUp_To_InTransit_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.PickedUp,
            ShipmentStatus.InTransit))
            .Should().NotThrow();
    }

    [Fact]
    public void ValidateTransition_InTransit_To_ArrivedAtFacility_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.InTransit,
            ShipmentStatus.ArrivedAtFacility))
            .Should().NotThrow();
    }

    [Fact]
    public void ValidateTransition_ArrivedAtFacility_To_OutForDelivery_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.ArrivedAtFacility,
            ShipmentStatus.OutForDelivery))
            .Should().NotThrow();
    }

    [Fact]
    public void ValidateTransition_OutForDelivery_To_Delivered_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.OutForDelivery,
            ShipmentStatus.Delivered))
            .Should().NotThrow();
    }

    #endregion

    #region Delayed Status Transitions

    [Fact]
    public void ValidateTransition_InTransit_To_Delayed_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.InTransit,
            ShipmentStatus.Delayed))
            .Should().NotThrow();
    }

    [Fact]
    public void ValidateTransition_ArrivedAtFacility_To_Delayed_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.ArrivedAtFacility,
            ShipmentStatus.Delayed))
            .Should().NotThrow();
    }

    [Fact]
    public void ValidateTransition_OutForDelivery_To_Delayed_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.OutForDelivery,
            ShipmentStatus.Delayed))
            .Should().NotThrow();
    }

    [Fact]
    public void ValidateTransition_Delayed_To_InTransit_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.Delayed,
            ShipmentStatus.InTransit))
            .Should().NotThrow();
    }

    [Fact]
    public void ValidateTransition_Delayed_To_OutForDelivery_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.Delayed,
            ShipmentStatus.OutForDelivery))
            .Should().NotThrow();
    }

    #endregion

    #region Exception Status Transitions

    [Fact]
    public void ValidateTransition_InTransit_To_Exception_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.InTransit,
            ShipmentStatus.Exception))
            .Should().NotThrow();
    }

    [Fact]
    public void ValidateTransition_ArrivedAtFacility_To_Exception_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.ArrivedAtFacility,
            ShipmentStatus.Exception))
            .Should().NotThrow();
    }

    [Fact]
    public void ValidateTransition_OutForDelivery_To_Exception_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.OutForDelivery,
            ShipmentStatus.Exception))
            .Should().NotThrow();
    }

    [Fact]
    public void ValidateTransition_Exception_To_InTransit_Succeeds()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.Exception,
            ShipmentStatus.InTransit))
            .Should().NotThrow();
    }

    #endregion

    #region Invalid Transitions

    [Fact]
    public void ValidateTransition_Created_To_InTransit_Fails()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.Created,
            ShipmentStatus.InTransit))
            .Should().Throw<InvalidShipmentTransitionException>();
    }

    [Fact]
    public void ValidateTransition_Created_To_Delayed_Fails()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.Created,
            ShipmentStatus.Delayed))
            .Should().Throw<InvalidShipmentTransitionException>();
    }

    [Fact]
    public void ValidateTransition_PickedUp_To_ArrivedAtFacility_Fails()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.PickedUp,
            ShipmentStatus.ArrivedAtFacility))
            .Should().Throw<InvalidShipmentTransitionException>();
    }

    [Fact]
    public void ValidateTransition_PickedUp_To_Delayed_Fails()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.PickedUp,
            ShipmentStatus.Delayed))
            .Should().Throw<InvalidShipmentTransitionException>();
    }

    [Fact]
    public void ValidateTransition_Delayed_To_Delayed_Fails()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.Delayed,
            ShipmentStatus.Delayed))
            .Should().Throw<InvalidShipmentTransitionException>();
    }

    [Fact]
    public void ValidateTransition_Exception_To_OutForDelivery_Fails()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.Exception,
            ShipmentStatus.OutForDelivery))
            .Should().Throw<InvalidShipmentTransitionException>();
    }

    [Fact]
    public void ValidateTransition_Exception_To_Delayed_Fails()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.Exception,
            ShipmentStatus.Delayed))
            .Should().Throw<InvalidShipmentTransitionException>();
    }

    [Fact]
    public void ValidateTransition_Delivered_To_InTransit_Fails()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.Delivered,
            ShipmentStatus.InTransit))
            .Should().Throw<InvalidShipmentTransitionException>()
            .WithMessage("*Delivered is a terminal state*");
    }

    [Fact]
    public void ValidateTransition_Delivered_To_Delivered_Fails()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.Delivered,
            ShipmentStatus.Delivered))
            .Should().Throw<InvalidShipmentTransitionException>();
    }

    #endregion

    #region Duplicate Status

    [Fact]
    public void ValidateTransition_SameStatus_Fails()
    {
        _validator.Invoking(v => v.ValidateTransition(
            ShipmentStatus.InTransit,
            ShipmentStatus.InTransit))
            .Should().Throw<InvalidShipmentTransitionException>()
            .WithMessage("*Status is already set to this value*");
    }

    #endregion

    #region Terminal State (Delivered)

    [Fact]
    public void ValidateTransition_Delivered_To_Any_Status_Fails()
    {
        var validStatusValues = Enum.GetValues(typeof(ShipmentStatus))
            .Cast<ShipmentStatus>()
            .Where(s => s != ShipmentStatus.Delivered)
            .ToList();

        foreach (var status in validStatusValues)
        {
            _validator.Invoking(v => v.ValidateTransition(
                ShipmentStatus.Delivered,
                status))
                .Should().Throw<InvalidShipmentTransitionException>()
                .WithMessage("*Delivered is a terminal state*");
        }
    }

    #endregion

    #region Error Message Validation

    [Fact]
    public void ValidateTransition_InvalidTransition_ContainsCurrentAndNewStatus()
    {
        var exception = Assert.Throws<InvalidShipmentTransitionException>(
            () => _validator.ValidateTransition(
                ShipmentStatus.Created,
                ShipmentStatus.InTransit));

        exception.Message.Should()
            .Contain("Created")
            .And.Contain("InTransit");
    }

    [Fact]
    public void ValidateTransition_DeliveredTerminalState_ContainsExplicitMessage()
    {
        var exception = Assert.Throws<InvalidShipmentTransitionException>(
            () => _validator.ValidateTransition(
                ShipmentStatus.Delivered,
                ShipmentStatus.InTransit));

        exception.Message.Should().Contain("terminal state");
    }

    #endregion
}
