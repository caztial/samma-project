import { useState, useEffect, useRef, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import {
  Heading,
  Text,
  StatusLight,
  IllustratedMessage,
  ProgressCircle,
  ToastQueue,
  Meter,
  ActionButton,
  Badge,
  Button,
  Divider,
} from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import ProfileLayout from '../../layouts/ProfileLayout';
import { useAuth } from '../../contexts/AuthContext';
import { createSessionService } from '../../services/sessionService';
import { createQuestionService } from '../../services/questionService';
import { getCurrentSession, saveAnswerState, getAnswerState } from '../../services/sessionStorage';
import { createSignalRService, ConnectionState } from '../../services/signalrService';
import Location from '@react-spectrum/s2/icons/Location';
import Clock from '@react-spectrum/s2/icons/Clock';
import ChevronLeft from '@react-spectrum/s2/icons/ChevronLeft';
import ChevronRight from '@react-spectrum/s2/icons/ChevronRight';

// ─── Static styles (S2 style macro — must be module-level constants) ──────────

const containerStyle = style({
  display: 'flex',
  flexDirection: 'column',
  flexGrow: 1,
  padding: 16,
  minWidth: 0,
  gap: 16,
});

const headerRowStyle = style({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  gap: 12,
});

const infoRowStyle = style({
  display: 'flex',
  flexWrap: 'wrap',
  gap: 16,
  color: 'neutral-subdued',
  font: 'body-sm',
});

const infoItemStyle = style({
  display: 'flex',
  alignItems: 'center',
  gap: 4,
});

const meterRowStyle = style({
  width: 'full',
});

const navRowStyle = style({
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'space-between',
  gap: 8,
});

const navLabelStyle = style({
  font: 'body-sm',
  color: 'neutral-subdued',
  textAlign: 'center',
  flexGrow: 1,
});

// Question card — custom styled container (not S2 Card, which is for navigable objects)
const questionCardStyle = style({
  backgroundColor: 'layer-1',
  borderRadius: 'lg',
  boxShadow: 'elevated',
  padding: 20,
  display: 'flex',
  flexDirection: 'column',
  gap: 16,
});

const cardHeaderRowStyle = style({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  gap: 8,
});

const questionNumberStyle = style({
  font: 'title-sm',
  color: 'neutral-subdued',
  fontWeight: 'bold',
});

const cardMetaRowStyle = style({
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'space-between',
  gap: 8,
  flexWrap: 'wrap',
});

const questionTitleStyle = style({
  font: 'heading-sm',
});

const questionDescStyle = style({
  font: 'body-sm',
  color: 'neutral-subdued',
});

const optionsListStyle = style({
  display: 'flex',
  flexDirection: 'column',
  gap: 8,
});

// Option row — unselected state
const optionRowStyle = style({
  display: 'flex',
  alignItems: 'center',
  gap: 12,
  paddingX: 12,
  paddingY: 12,
  backgroundColor: 'layer-2',
  borderRadius: 'default',
  cursor: 'pointer',
});

// Option row — selected state
const optionRowSelectedStyle = style({
  display: 'flex',
  alignItems: 'center',
  gap: 12,
  paddingX: 12,
  paddingY: 12,
  backgroundColor: 'accent-subtle',
  borderRadius: 'default',
  cursor: 'pointer',
});

// Option row — disabled (after submit or timeout)
const optionRowDisabledStyle = style({
  display: 'flex',
  alignItems: 'center',
  gap: 12,
  paddingX: 12,
  paddingY: 12,
  backgroundColor: 'gray-100',
  borderRadius: 'default',
  cursor: 'default',
});

const optionLetterStyle = style({
  font: 'body-sm',
  fontWeight: 'bold',
  color: 'accent-900',
  minWidth: 16,
  textAlign: 'center',
});

const optionTextStyle = style({
  font: 'body',
  flexGrow: 1,
});

const submitRowStyle = style({
  paddingTop: 4,
});

const submitButtonStyle = style({
  width: 'full',
});

const waitingContainerStyle = style({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
  flexGrow: 1,
  gap: 16,
  minHeight: 300,
});

const loadingContainerStyle = style({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
  flexGrow: 1,
  gap: 16,
});

const errorContainerStyle = style({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
  flexGrow: 1,
  gap: 16,
  padding: 32,
});

// ─── Helpers ──────────────────────────────────────────────────────────────────

function formatStartedTime(dateString) {
  if (!dateString) return '';
  const date = new Date(dateString);
  const now = new Date();
  const isToday = date.toDateString() === now.toDateString();
  if (isToday) {
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }
  return date.toLocaleDateString([], {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

/** Convert index (0-based) to option letter: 0→A, 1→B, … */
function indexToLetter(index) {
  return String.fromCharCode(65 + index);
}

/** Compute seconds remaining from activatedAt UTC and durationSeconds */
function computeTimeLeft(activatedAt, durationSeconds) {
  if (!activatedAt || !durationSeconds) return null;
  const elapsed = (Date.now() - new Date(activatedAt).getTime()) / 1000;
  return Math.max(0, Math.floor(durationSeconds - elapsed));
}

/** Get Badge variant based on seconds left */
function timerVariant(seconds) {
  if (seconds <= 10) return 'negative';
  if (seconds <= 30) return 'notice';
  return 'informative';
}

/**
 * Composite key for timedOutSet: "questionId:attemptNumber"
 * Ensures a new attempt gets a fresh timer even if the previous attempt timed out.
 */
function timedOutKey(questionId, attemptNumber) {
  return `${questionId}:${attemptNumber}`;
}

// ─── McqQuestionCard ──────────────────────────────────────────────────────────

/**
 * Renders a single MCQ question card with options, timer, attempt indicator, and submit.
 */
function McqQuestionCard({
  question,
  selectedOptionId,
  isSubmitted,
  isTimedOut,
  isSubmitting,
  timeLeft,
  onSelectOption,
  onSubmit,
  t,
}) {
  const { activeAttempt, maxAttempts, showTitle, showOptionValues } = question;

  // Sort options by order ascending
  const sortedOptions = [...question.options].sort((a, b) => a.order - b.order);

  const canInteract = !isSubmitted && !isTimedOut;
  const canSubmit = canInteract && !!selectedOptionId && !isSubmitting;

  const attemptDisplay =
    activeAttempt
      ? t('profile.content.sessions.active.attempt', {
        current: activeAttempt.attemptNumber,
        max: maxAttempts,
      })
      : null;

  const timerDisplay =
    timeLeft !== null
      ? isTimedOut
        ? t('profile.content.sessions.active.timesUp')
        : t('profile.content.sessions.active.timeLeft', { seconds: timeLeft })
      : null;

  return (
    <div className={questionCardStyle}>
      {/* Card header: Question Number + Attempt indicator */}
      <div className={cardHeaderRowStyle}>
        <Text styles={questionNumberStyle}>{question.questionNumber}</Text>
        {attemptDisplay && (
          <Badge variant="informative" size="S">
            {attemptDisplay}
          </Badge>
        )}
      </div>

      {/* Timer row */}
      {timerDisplay && (
        <div className={cardMetaRowStyle}>
          <Badge variant={isTimedOut ? 'negative' : timerVariant(timeLeft ?? 0)} size="S">
            <Clock />
            <Text>{timerDisplay}</Text>
          </Badge>
        </div>
      )}

      {/* Question title + description — only if showTitle */}
      {showTitle && question.questionText && (
        <Heading level={4} styles={questionTitleStyle}>
          {question.questionText}
        </Heading>
      )}
      {showTitle && question.questionDescription && (
        <Text styles={questionDescStyle}>{question.questionDescription}</Text>
      )}

      <Divider size="S" />

      {/* MCQ Options */}
      <div className={optionsListStyle}>
        {sortedOptions.map((option, idx) => {
          const isSelected = selectedOptionId === option.optionId;
          let rowClass = optionRowStyle;
          if (!canInteract) {
            rowClass = optionRowDisabledStyle;
          } else if (isSelected) {
            rowClass = optionRowSelectedStyle;
          }

          return (
            <div
              key={option.optionId}
              className={rowClass}
              role="button"
              tabIndex={canInteract ? 0 : -1}
              aria-pressed={isSelected}
              onClick={() => canInteract && onSelectOption(option.optionId)}
              onKeyDown={(e) => {
                if (canInteract && (e.key === 'Enter' || e.key === ' ')) {
                  e.preventDefault();
                  onSelectOption(option.optionId);
                }
              }}
            >
              {!showOptionValues && (
                <Text styles={optionLetterStyle}>{option.optionNumber}</Text>
              )}
              {showOptionValues && (
                <Text styles={optionTextStyle}>{option.optionText}</Text>
              )}
            </div>
          );
        })}
      </div>

      {/* Submit button */}
      <div className={submitRowStyle}>
        {isSubmitted ? (
          <Badge variant="positive" size="M">
            {t('profile.content.sessions.active.submitted')}
          </Badge>
        ) : (
          <Button
            variant="accent"
            isPending={isSubmitting}
            isDisabled={!canSubmit}
            onPress={onSubmit}
            styles={submitButtonStyle}
          >
            <Text>
              {isSubmitting
                ? t('profile.content.sessions.active.submitting')
                : t('profile.content.sessions.active.submit')}
            </Text>
          </Button>
        )}
      </div>
    </div>
  );
}

// ─── ActiveSessionPage ────────────────────────────────────────────────────────

/**
 * ActiveSessionPage - Page for active session participation.
 *
 * Features:
 * - Fetches and displays presented MCQ questions from GET /sessions/{id}/presented
 * - Meter shows X/Y unique questions answered (at least one attempt submitted)
 * - Multi-question navigation with left/right ActionButton arrows
 * - Per-question countdown timer — runs continuously regardless of submission state
 * - Timer is keyed per attempt ("qId:attemptNum") so a new attempt resets the timeout
 * - MCQ option selection with visual highlight
 * - Submit via POST /sessions/{sid}/questions/{qid}/attempts/{n}/answers
 * - Submitted answers tracked per attempt: { [qId]: { [attemptNumber]: optionId } }
 */
export default function ActiveSessionPage() {
  const { sessionId } = useParams();
  const { t } = useTranslation();
  const { getToken, onUnauthorized } = useAuth();

  // ── Session info ────────────────────────────────────────────────────────────
  const [session, setSession] = useState(null);
  const [isSessionLoading, setIsSessionLoading] = useState(true);
  const [sessionError, setSessionError] = useState(null);

  // ── Questions ───────────────────────────────────────────────────────────────
  const [questions, setQuestions] = useState([]);
  const [isQuestionsLoading, setIsQuestionsLoading] = useState(true);
  const [currentIndex, setCurrentIndex] = useState(0);

  // ── Per-question interaction state ──────────────────────────────────────────
  // { [questionId]: optionId } — currently selected (or last selected) option per question
  const [selectedOptions, setSelectedOptions] = useState({});

  // { [questionId]: { [attemptNumber]: optionId } } — confirmed submissions keyed by attempt
  // A question is only "submitted for current attempt" if submittedAnswers[qId][attemptNumber] exists.
  // This allows re-answering when a new attempt is presented.
  const [submittedAnswers, setSubmittedAnswers] = useState({});

  // Set of composite keys "questionId:attemptNumber" where timer has expired.
  // Using composite key means a new attempt gets a fresh timer even if prior attempt timed out.
  const [timedOutSet, setTimedOutSet] = useState(new Set());

  // { [questionId]: seconds }
  const [timeLeftMap, setTimeLeftMap] = useState({});

  // Set of questionIds currently mid-submission (network in flight)
  const [submittingSet, setSubmittingSet] = useState(new Set());

  // SignalR connection status
  const [connectionStatus, setConnectionStatus] = useState(ConnectionState.DISCONNECTED);

  // ── Services (created once via refs) ────────────────────────────────────────
  const sessionServiceRef = useRef(null);
  const questionServiceRef = useRef(null);
  const signalRServiceRef = useRef(null);

  if (!sessionServiceRef.current) {
    sessionServiceRef.current = createSessionService({ getToken: () => getToken(), onUnauthorized });
  }
  if (!questionServiceRef.current) {
    questionServiceRef.current = createQuestionService({ getToken: () => getToken(), onUnauthorized });
  }
  if (!signalRServiceRef.current) {
    signalRServiceRef.current = createSignalRService({
      getToken: () => getToken(),
      onConnected: () => setConnectionStatus(ConnectionState.CONNECTED),
      onDisconnected: () => setConnectionStatus(ConnectionState.DISCONNECTED),
      onReconnecting: () => setConnectionStatus(ConnectionState.RECONNECTING),
      onReconnected: () => setConnectionStatus(ConnectionState.CONNECTED),
    });
  }

  // ── Fetch session info ───────────────────────────────────────────────────────
  useEffect(() => {
    let isMounted = true;

    const fetchSession = async () => {
      setIsSessionLoading(true);
      setSessionError(null);
      try {
        // Hydrate from cache first for instant display
        const cached = getCurrentSession();
        if (cached && cached.sessionId === sessionId && isMounted) {
          setSession({
            id: cached.sessionId,
            name: cached.sessionName,
            code: cached.sessionCode,
            state: cached.sessionState,
            startedAt: cached.joinedAt,
          });
        }
        // Then fetch fresh data
        const fresh = await sessionServiceRef.current.getSession(sessionId);
        if (isMounted) setSession(fresh);
      } catch (err) {
        if (isMounted) {
          setSessionError(
            err.response?.status === 404
              ? t('profile.content.sessions.detail.notFound')
              : t('profile.content.sessions.detail.loadError')
          );
        }
      } finally {
        if (isMounted) setIsSessionLoading(false);
      }
    };

    if (sessionId) fetchSession();
    return () => { isMounted = false; };
  }, [sessionId]);

  // ── Fetch presented questions + hydrate persisted answer state ───────────────
  useEffect(() => {
    let isMounted = true;

    const fetchQuestions = async () => {
      setIsQuestionsLoading(true);
      try {
        const data = await questionServiceRef.current.getPresentedQuestions(sessionId);
        if (isMounted) {
          setQuestions(data);
          setCurrentIndex(0);

          // Build initial timeLeftMap AND pre-seed timedOutSet for already-elapsed questions
          const initialTimeLeft = {};
          const initialTimedOut = new Set();

          data.forEach((q) => {
            if (q.activeAttempt?.activatedAt && q.durationSeconds) {
              const remaining = computeTimeLeft(q.activeAttempt.activatedAt, q.durationSeconds);
              initialTimeLeft[q.questionId] = remaining;
              if (remaining === 0) {
                // Use composite key so new attempts start fresh
                initialTimedOut.add(timedOutKey(q.questionId, q.activeAttempt.attemptNumber));
              }
            }
          });

          setTimeLeftMap(initialTimeLeft);
          if (initialTimedOut.size > 0) {
            setTimedOutSet(initialTimedOut);
          }

          // Hydrate persisted answer state (survives navigation / page reload)
          const saved = getAnswerState(sessionId);
          if (saved) {
            setSelectedOptions(saved.selectedOptions ?? {});
            setSubmittedAnswers(saved.submittedAnswers ?? {});
          }
        }
      } catch (err) {
        // Non-fatal — if questions fail, show waiting state
        if (isMounted) setQuestions([]);
      } finally {
        if (isMounted) setIsQuestionsLoading(false);
      }
    };

    if (sessionId) fetchQuestions();
    return () => { isMounted = false; };
  }, [sessionId]);

  // ── Countdown ticker ─────────────────────────────────────────────────────────
  // Timer always runs regardless of submission state — a submitted question still
  // shows the elapsed countdown for the user's reference.
  useEffect(() => {
    if (questions.length === 0) return;

    const interval = setInterval(() => {
      setTimeLeftMap((prev) => {
        const next = { ...prev };
        let changed = false;

        questions.forEach((q) => {
          if (!q.activeAttempt?.activatedAt || !q.durationSeconds) return;

          const remaining = computeTimeLeft(q.activeAttempt.activatedAt, q.durationSeconds);
          if (prev[q.questionId] !== remaining) {
            next[q.questionId] = remaining;
            changed = true;

            if (remaining === 0) {
              const key = timedOutKey(q.questionId, q.activeAttempt.attemptNumber);
              setTimedOutSet((ts) => {
                if (ts.has(key)) return ts;
                return new Set(ts).add(key);
              });
            }
          }
        });

        return changed ? next : prev;
      });
    }, 500);

    return () => clearInterval(interval);
  }, [questions]);

  // ── SignalR connection ───────────────────────────────────────────────────────
  // Connect to session hub and join session group when session is loaded
  useEffect(() => {
    if (!session?.id) return;

    let isMounted = true;

    const connectToHub = async () => {
      try {
        setConnectionStatus(ConnectionState.CONNECTING);
        await signalRServiceRef.current.connect();

        if (isMounted) {
          await signalRServiceRef.current.joinSessionGroup(session.id);
        }
      } catch (error) {
        console.error('Failed to connect to SignalR hub:', error);
        if (isMounted) {
          setConnectionStatus(ConnectionState.DISCONNECTED);
        }
      }
    };

    connectToHub();

    return async () => {
      isMounted = false;
      try {
        await signalRServiceRef.current.leaveSessionGroup(session.id);
        await signalRServiceRef.current.disconnect();
      } catch (error) {
        // Ignore errors on cleanup
      }
    };
  }, [session?.id]);

  // ── Handlers ─────────────────────────────────────────────────────────────────

  const handleSelectOption = useCallback((questionId, optionId) => {
    setSelectedOptions((prev) => ({ ...prev, [questionId]: optionId }));
  }, []);

  const handleSubmit = useCallback(
    async (question) => {
      const { questionId, activeAttempt } = question;
      if (!activeAttempt) return;

      const selectedOptionId = selectedOptions[questionId];
      if (!selectedOptionId) return;

      // Mark as submitting
      setSubmittingSet((prev) => new Set(prev).add(questionId));

      try {
        await questionServiceRef.current.submitAnswer(
          sessionId,
          questionId,
          activeAttempt.attemptNumber,
          selectedOptionId
        );

        // Record the submitted answer against this specific attempt number
        setSubmittedAnswers((prev) => {
          const updated = {
            ...prev,
            [questionId]: {
              ...(prev[questionId] ?? {}),
              [activeAttempt.attemptNumber]: selectedOptionId,
            },
          };

          // Persist to localStorage so state survives navigation/reload
          // We capture updated here inside the setState callback to get the latest value
          saveAnswerState(sessionId, {
            selectedOptions,
            submittedAnswers: updated,
          });

          return updated;
        });

        ToastQueue.positive(t('profile.content.sessions.active.submitSuccess'), {
          timeout: 4000,
        });
      } catch (err) {
        const msg =
          err.response?.data?.error ||
          t('profile.content.sessions.active.submitError');
        ToastQueue.negative(msg, { timeout: 5000 });
      } finally {
        setSubmittingSet((prev) => {
          const next = new Set(prev);
          next.delete(questionId);
          return next;
        });
      }
    },
    [selectedOptions, sessionId, t]
  );

  // ── Status light variant ──────────────────────────────────────────────────────
  const getStatusVariant = () => {
    switch (connectionStatus) {
      case ConnectionState.CONNECTED: return 'positive';
      case ConnectionState.CONNECTING:
      case ConnectionState.RECONNECTING: return 'notice';
      default: return 'negative';
    }
  };

  // ── Loading / Error states ────────────────────────────────────────────────────
  if (isSessionLoading) {
    return (
      <ProfileLayout>
        <div className={loadingContainerStyle}>
          <ProgressCircle size="L" isIndeterminate />
          <Text>{t('common.loading')}</Text>
        </div>
      </ProfileLayout>
    );
  }

  if (sessionError || !session) {
    return (
      <ProfileLayout>
        <div className={errorContainerStyle}>
          <IllustratedMessage>
            <Text>{sessionError || t('profile.content.sessions.detail.notFound')}</Text>
          </IllustratedMessage>
        </div>
      </ProfileLayout>
    );
  }

  // ── Derived values ────────────────────────────────────────────────────────────
  const currentQuestion = questions[currentIndex] ?? null;

  // Count of unique questions that have been answered at least once across any attempt
  const attemptedCount = Object.keys(submittedAnswers).length;

  // For the current question + active attempt: is this specific attempt already submitted?
  const currentAttemptNumber = currentQuestion?.activeAttempt?.attemptNumber;
  const isCurrentSubmitted =
    currentQuestion != null &&
    currentAttemptNumber != null &&
    submittedAnswers[currentQuestion.questionId]?.[currentAttemptNumber] != null;

  // Is the current attempt's timer expired?
  const isCurrentTimedOut =
    currentQuestion != null &&
    currentAttemptNumber != null &&
    timedOutSet.has(timedOutKey(currentQuestion.questionId, currentAttemptNumber));

  // ── Render ────────────────────────────────────────────────────────────────────
  return (
    <ProfileLayout>
      <div className={containerStyle}>

        {/* ── Session Header ── */}
        <div className={headerRowStyle}>
          <Heading level={3}>
            {session.name}
          </Heading>
          <StatusLight variant={getStatusVariant()}>
            {t(`profile.content.sessions.active.connectionStatus.${connectionStatus}`)}
          </StatusLight>
        </div>

        {/* ── Info row: Location + Started time ── */}
        {(session.location || session.startedAt) && (
          <div className={infoRowStyle}>
            {session.location && (
              <div className={infoItemStyle}>
                <Location />
                <Text>{session.location}</Text>
              </div>
            )}
            {session.startedAt && (
              <div className={infoItemStyle}>
                <Clock />
                <Text>
                  {t('profile.content.sessions.active.startedAt', {
                    time: formatStartedTime(session.startedAt),
                  })}
                </Text>
              </div>
            )}
          </div>
        )}

        {/* ── Meter: unique questions answered / total ── */}
        {questions.length > 0 && (
          <div className={meterRowStyle}>
            <Meter
              label={t('profile.content.sessions.active.meter.label')}
              value={attemptedCount}
              minValue={0}
              maxValue={questions.length}
              valueLabel={`${attemptedCount}/${questions.length}`}
              formatOptions={{ style: 'decimal' }}
              variant={attemptedCount === questions.length ? 'positive' : 'informative'}
            />
          </div>
        )}

        {/* ── Question navigation (only when >1 question) ── */}
        {questions.length > 1 && (
          <div className={navRowStyle}>
            <ActionButton
              isQuiet
              size="S"
              onPress={() => setCurrentIndex((i) => Math.max(0, i - 1))}
              isDisabled={currentIndex === 0}
              aria-label="Previous question"
            >
              <ChevronLeft />
            </ActionButton>

            <Text styles={navLabelStyle}>
              {t('profile.content.sessions.active.questionOf', {
                current: currentIndex + 1,
                total: questions.length,
              })}
            </Text>

            <ActionButton
              isQuiet
              size="S"
              onPress={() => setCurrentIndex((i) => Math.min(questions.length - 1, i + 1))}
              isDisabled={currentIndex === questions.length - 1}
              aria-label="Next question"
            >
              <ChevronRight />
            </ActionButton>
          </div>
        )}

        {/* ── Question card or waiting state ── */}
        {isQuestionsLoading ? (
          <div className={waitingContainerStyle}>
            <ProgressCircle size="M" isIndeterminate />
            <Text>{t('common.loading')}</Text>
          </div>
        ) : currentQuestion ? (
          <McqQuestionCard
            key={`${currentQuestion.questionId}:${currentAttemptNumber}`}
            question={currentQuestion}
            selectedOptionId={selectedOptions[currentQuestion.questionId] ?? null}
            isSubmitted={isCurrentSubmitted}
            isTimedOut={isCurrentTimedOut}
            isSubmitting={submittingSet.has(currentQuestion.questionId)}
            timeLeft={timeLeftMap[currentQuestion.questionId] ?? null}
            onSelectOption={(optionId) =>
              handleSelectOption(currentQuestion.questionId, optionId)
            }
            onSubmit={() => handleSubmit(currentQuestion)}
            t={t}
          />
        ) : (
          <div className={waitingContainerStyle}>
            <IllustratedMessage>
              <Text>{t('profile.content.sessions.active.waitingForQuestion')}</Text>
            </IllustratedMessage>
          </div>
        )}
      </div>
    </ProfileLayout>
  );
}
