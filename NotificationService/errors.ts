import { NextFunction, Request, Response } from 'express'

export class EntityNotFoundError extends Error {
  constructor(message: string) {
    super(message)
    this.name = 'EntityNotFoundError'
  }
}

export function validation(err: any, req: Request, res: Response, next: NextFunction) {
  if (err.name === 'ValidationError') {
    res.status(400).json({
      message: err.message
    })
  } else {
    next(err)
  }
}

export function cast(err: any, req: Request, res: Response, next: NextFunction) {
  if (err.name === 'CastError') {
    res.status(400).json({
      message: `Invalid resource id: ${err.value}`
      // message: err
    })
  } else {
    next(err)
  }
}

export function entityNotFound(err: any, req: Request, res: Response, next: NextFunction) {
  if (err.name === 'EntityNotFoundError') {
    res.status(404).json({
      message: err.message
    })
  } else {
    next(err)
  }
}