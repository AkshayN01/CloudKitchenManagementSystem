﻿using AutoMapper;
using CKMS.Contracts.Configuration;
using CKMS.Contracts.DTOs.AdminUser.Request;
using CKMS.Contracts.DTOs.Order.Request;
using CKMS.Interfaces.HttpClientServices;
using CKMS.Interfaces.Repository;
using CKMS.Interfaces.Storage;
using CKMS.OrderService.Blanket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CKMS.OrderService.API.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application _appSettings;
        private readonly OrderBlanket _orderBlanket;
        public OrderController(IOrderUnitOfWork unitOfWork, IMapper mapper, IOptions<Application> appSettings, IRedis redis, INotificationHttpService notificationHttpService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _orderBlanket = new OrderBlanket(unitOfWork, mapper, redis, notificationHttpService);
        }

        [HttpPost]
        [Authorize]
        [Route("/api/order/add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] OrderPayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _orderBlanket.AddToCart(payload, userguid);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/api/order/confirm-order")]
        public async Task<IActionResult> ConfirmOrder([FromBody] ConfirmOrderPayload payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }


            try
            {
                var httpResponse = await _orderBlanket.ConfirmOrder(userguid, payload);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/api/order/cancel-order/{orderId}")]
        public async Task<IActionResult> CancelOrder([FromRoute] String orderId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }


            try
            {
                var httpResponse = await _orderBlanket.CancelOrder(userguid, orderId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/order/get-cart")]
        public async Task<IActionResult> GetCart()
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            try
            {
                var httpResponse = await _orderBlanket.GetCart(userguid);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/order/view-order/{orderId}")]
        public async Task<IActionResult> ViewOrder([FromRoute] String orderId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }


            try
            {
                var httpResponse = await _orderBlanket.ViewOrder(orderId, userguid);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/order/view-all-orders")]
        public async Task<IActionResult> ViewOrders([FromQuery] int pageSize, int pageNumber)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }


            try
            {
                var httpResponse = await _orderBlanket.ViewAllOrder(userguid, pageSize, pageNumber);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #region " Business Owner "

        [HttpPut]
        [Authorize]
        [Route("/api/order/kitchen/update-order/{orderId}")]
        public async Task<IActionResult> UpdateOrder([FromRoute] String orderId, [FromBody] String status)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            var kitchenId = claims.FirstOrDefault(c => c.Type == "kitchenId")?.Value;
            if (kitchenId == null)
                return Unauthorized();

            try
            {
                var httpResponse = await _orderBlanket.UpdateOrder(orderId, status, kitchenId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/order/kitchen/view-order/{orderId}")]
        public async Task<IActionResult> ViewKitchenOrder([FromRoute] String orderId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            var kitchenId = claims.FirstOrDefault(c => c.Type == "kitchenId")?.Value;
            if(kitchenId == null)
                return Unauthorized();

            try
            {
                var httpResponse = await _orderBlanket.ViewKitchenOrder(kitchenId, orderId);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/order/kitchen/view-all-orders")]
        public async Task<IActionResult> ViewAllKitchenOrders([FromQuery] int pageSize, [FromQuery]int pageNumber, [FromQuery]String? status)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };
            var claims = User.Claims;
            var userguid = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userguid == null) { return Unauthorized(); }

            var kitchenId = claims.FirstOrDefault(c => c.Type == "kitchenId")?.Value;
            if (kitchenId == null)
                return Unauthorized();
            try
            {
                var httpResponse = await _orderBlanket.ViewAllKitchenOrder(kitchenId, status, pageSize, pageNumber);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #endregion
    }
}
