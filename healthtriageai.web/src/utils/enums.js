export function levelToKey(level) {
    if (level === null || level === undefined) return undefined
    if (typeof level === 'number') {
        if (level === 0) return 'low'
        if (level === 1) return 'moderate'
        if (level === 2) return 'high'
        if (level === 3) return 'critical'
        return undefined
    }
    const lv = String(level).toLowerCase()
    if (['low', 'moderate', 'high', 'critical'].includes(lv)) return lv
    return undefined
}
export function levelToLabel(level) {
    const k = levelToKey(level)
    if (k === 'low') return 'Low'
    if (k === 'moderate') return 'Moderate'
    if (k === 'high') return 'High'
    if (k === 'critical') return 'Critical'
    return 'Pending'
}
export function specialistToLabel(spec) {
    if (spec === null || spec === undefined) return 'N/A'
    if (typeof spec === 'number') {
        switch (spec) {
            case 0:
                return 'General Practice'
            case 1:
                return 'Cardiology'
            case 2:
                return 'Pulmonology'
            case 3:
                return 'Infectious Diseases'
            case 4:
                return 'Neurology'
            case 5:
                return 'Emergency'
            default:
                return 'N/A'
        }
    }
    return String(spec)
}