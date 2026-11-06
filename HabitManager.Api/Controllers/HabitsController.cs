using HabitManager.Api.Mapping;
using HabitManager.Application.Services;
using HabitManager.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace HabitManager.Api.Controllers;

[ApiController]
[Route("api/habits")]
public class HabitsController : ControllerBase
{
    // Pagaidu lietotājs līdz autentifikācijas ieviešanai.
    private const string CurrentUserId = "demo-user-1";

    private readonly HabitApplicationService _service;

    public HabitsController(HabitApplicationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<HabitDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var habits = await _service.GetAllAsync(
            CurrentUserId, cancellationToken);

        return Ok(habits.Select(HabitApiMapper.ToDto).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<HabitDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var habit = await _service.GetByIdAsync(
            id, CurrentUserId, cancellationToken);

        return Ok(HabitApiMapper.ToDto(habit));
    }

    [HttpPost]
    public async Task<ActionResult<HabitDto>> Create(
        [FromBody] CreateHabitRequest request,
        CancellationToken cancellationToken)
    {
        var schedule = HabitApiMapper.ToDomain(request.Schedule);

        var habit = await _service.CreateAsync(
            CurrentUserId,
            request.Name,
            request.Description,
            schedule,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = habit.Id },
            HabitApiMapper.ToDto(habit));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<HabitDto>> Update(
        Guid id,
        [FromBody] UpdateHabitRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Id != Guid.Empty && request.Id != id)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Habit identifiers do not match."
            });
        }

        var schedule = HabitApiMapper.ToDomain(request.Schedule);

        var habit = await _service.UpdateAsync(
            id,
            CurrentUserId,
            request.Name,
            request.Description,
            schedule,
            cancellationToken);

        return Ok(HabitApiMapper.ToDto(habit));
    }

    [HttpPut("{id:guid}/archive")]
    public async Task<IActionResult> Archive(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _service.SetArchivedAsync(
            id, CurrentUserId, true, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}/archive")]
    public async Task<IActionResult> Unarchive(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _service.SetArchivedAsync(
            id, CurrentUserId, false, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:guid}/completions")]
    public async Task<ActionResult<List<HabitCompletionDto>>> GetCompletions(
        Guid id,
        CancellationToken cancellationToken)
    {
        var completions = await _service.GetCompletionsAsync(
            id, CurrentUserId, cancellationToken);

        return Ok(completions.Select(HabitApiMapper.ToDto).ToList());
    }

    [HttpPost("{id:guid}/completions")]
    public async Task<IActionResult> Complete(
        Guid id,
        [FromBody] HabitCompletionRequest request,
        CancellationToken cancellationToken)
    {
        if (request.HabitId != Guid.Empty && request.HabitId != id)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Habit identifiers do not match."
            });
        }

        if (request.Date == default)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Completion date is required."
            });
        }

        await _service.CompleteAsync(
            id, CurrentUserId, request.Date, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}/completions/{date}")]
    public async Task<IActionResult> UndoCompletion(
        Guid id,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        await _service.UndoCompletionAsync(
            id, CurrentUserId, date, cancellationToken);

        return NoContent();
    }
}