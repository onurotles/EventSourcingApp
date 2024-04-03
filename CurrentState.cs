namespace EventSourcingApp {
    public class CurrentState {
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public DateTime LastReceived { get; set; }
        public DateTime LastShipped { get; set; }
    }
}