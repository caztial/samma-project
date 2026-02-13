# Project Brief: Dhamma Session Management System

## Overview
A real-time platform for organizing Dhamma (Buddhist teaching) sessions where conductors can push questions to participants, track their understanding through answers, and manage devotees' information.

## Core Purpose
Enable Dhamma teachers to conduct interactive sessions with devotees, assess their understanding through structured questions, and maintain a history of their spiritual learning journey.

## Target Users
- **Participants (Devotees)**: Join sessions, answer questions, track progress
- **Moderators**: Assist conductors, manage question bank, push questions
- **Admins**: Full system management, user administration

## MVP Scope

### Features Included in MVP
1. **User Authentication**
   - ASP.NET Identity with JWT
   - Role-based access (Participant, Moderator, Admin)
   - Password reset with admin approval

2. **User Profiles**
   - Basic information
   - Contacts (phone, email, address)
   - Education details

3. **Question Bank**
   - MCQ questions with 1:N answer options
   - Mark correct answer(s)
   - Search by tags/text
   - Batch assign to sessions

4. **Session Management**
   - Create sessions with auto-generated codes
   - Session lifecycle: Draft → Active → Paused → Ended
   - Only active sessions are interactive
   - Join via code or QR scan

5. **Real-time Q&A**
   - SignalR for live communication
   - Push questions to participants
   - Multiple attempts (3 max) with decreasing points (100% → 50% → 25%)
   - Time limits between attempts

6. **Presentation Mode**
   - Projector-optimized view
   - Question display
   - Results visualization

7. **Scoring & Audit**
   - Points per session
   - Full audit logging

8. **Deployment**
   - Docker Compose
   - LAN/offline capable

## Success Criteria
- Participants can join sessions and answer questions in real-time
- Admins can manage question bank and sessions
- System works offline on local network
- Full audit trail of all activities

## Timeline
4-week MVP development with iterative feature delivery.

## Technology Stack
- **Backend**: .NET 10, ASP.NET Identity, SignalR, EF Core
- **Frontend**: React 18, Vite, TypeScript, Shopify Polaris Web Components
- **Database**: PostgreSQL
- **Deployment**: Docker Compose
